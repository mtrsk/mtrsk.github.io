{
  description = "Blog";

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
          
          tooling = with pkgs; [
            just
            hugo
          ];
          
          customEmacs = (pkgs.emacsPackagesFor pkgs.emacs-nox).emacsWithPackages (epkgs:
            with epkgs.melpaPackages; [
              ox-hugo
              citeproc
            ] ++ (with epkgs.elpaPackages; [
              org
            ]));
        in
        {
          # `nix develop .#ci`
          # reduce the number of packages to the bare minimum needed for CI
          ci = pkgs.mkShell {
            buildInputs = [ customEmacs ] ++ tooling;
          };

          # `nix develop`
          default = devenv.lib.mkShell {
            inherit inputs pkgs;
            modules = [
              ({ pkgs, lib, ... }: {
                packages = [ customEmacs ] ++ tooling;

                enterShell = ''
                  echo "Starting environment..."
                '';
              })
            ];
          };
        });
    };
}
