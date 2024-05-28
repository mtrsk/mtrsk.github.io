{
  description = "Development Environment";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    devenv = {
      url = "github:cachix/devenv";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = inputs@{ self, devenv, nixpkgs, ... }:
    let
      systems = [ "x86_64-linux" "x86_64-darwin" "aarch64-linux" "aarch64-darwin" ];
      forAllSystems = f: builtins.listToAttrs (map (name: { inherit name; value = f name; }) systems);
      mkPkgs = system: import nixpkgs {
        inherit system;
        config = {
          allowUnfree = true;
        };
      };
    in
    {
      devShells = forAllSystems (system:
        let
          pkgs = mkPkgs system;

          tooling = with pkgs; [
            just
          ];

          darwinPkgs = with pkgs.darwin.apple_sdk.frameworks; [
            CoreFoundation
            CoreServices
          ];

          development = with pkgs; [
            # .Net
            netcoredbg
            fsautocomplete
            fantomas
          ];

          dotnet = with pkgs.dotnetCorePackages; combinePackages [
            sdk_8_0
          ];
        in
        {
          # `nix develop .#ci`
          # reduce the number of packages to the bare minimum needed for CI
          ci = pkgs.mkShell {
            buildInputs = tooling ++ [ dotnet ];
          };

          # `nix develop`
          default = devenv.lib.mkShell {
            inherit inputs pkgs;
            modules = [
              ({ pkgs, lib, ... }: {
                packages =
                  tooling
                  ++ development
                  ++ lib.optionals pkgs.stdenv.isDarwin (darwinPkgs);

                languages.dotnet = {
                  enable = true;
                  package = dotnet;
                };

                scripts = {
                  build.exec = "just build";
                  clean.exec = "just clean";
                  watch.exec = "just watch";
                };

                env = {
                  DOTNET_ROOT = "${dotnet}";
                  DOTNET_CLI_TELEMETRY_OPTOUT = "1";
                };

                enterShell = ''
                  echo "==> Setting up development environment"
                  echo "==> Using fornax version: $(dotnet fornax version)"
                '';
              })
            ];
          };
        });
    };
  }
