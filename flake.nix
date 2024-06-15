{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    devenv = {
      url = "github:cachix/devenv";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = { self, nixpkgs, devenv, ... } @ inputs:
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

          linuxPkgs = with pkgs; [
          ];

          darwinPkgs = with pkgs.darwin.apple_sdk.frameworks; [
            CoreFoundation
            CoreServices
          ];

          tooling = with pkgs; [
            just
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
                  ++ lib.optionals pkgs.stdenv.isLinux (linuxPkgs)
                  ++ lib.optionals pkgs.stdenv.isDarwin darwinPkgs;

                env = {
                  DOTNET_ROOT = "${dotnet}";
                  DOTNET_CLI_TELEMETRY_OPTOUT = "1";
                  LANG = "en_US.UTF-8";
                };

                languages.dotnet = {
                  enable = true;
                  package = dotnet;
                };

                enterShell = ''
                  dotnet --version
                '';
              })
            ];
          };
        });
    };
}
