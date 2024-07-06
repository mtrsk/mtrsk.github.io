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

          texenv = pkgs.texlive.combine {
            inherit (pkgs.texlive)
              accsupp
              collection-basic
              collection-fontsextra
              collection-fontsrecommended
              collection-langenglish
              collection-langportuguese
              collection-latexextra
              collection-mathscience
              extsizes
              etoolbox
              hyphen-portuguese
              latexmk
              paracol
              pdfx
              ragged2e
              scheme-medium
              ;
          };
          
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
          # Reduce the number of packages to the bare minimum needed for CI,
          # by removing LaTeX and not using my own Emacs configuration, but
          # a custom package with just enough tools to generate the markdown
          # for org-roam.
          ci = pkgs.mkShell {
            buildInputs = [ customEmacs ] ++ tooling;
          };

          # `nix develop --impure`
          # This is the development shell, meant to be used as an impure
          # shell, so no custom Emacs here, just use your global package
          # switch back to the CI shell for builds.
          default = devenv.lib.mkShell {
            inherit inputs pkgs;
            modules = [
              ({ pkgs, lib, ... }: {
                packages = [ texenv ] ++ tooling;

                scripts = {
                  build.exec = "just build";
                  publish.exec = "just publish";
                  run.exec = "just run";
                  clean.exec = "just clean";
                };

                enterShell = ''
                  echo "Starting environment..."
                  hugo version
                '';
              })
            ];
          };
        });
    };
}
