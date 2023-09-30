{
  description = "Blog";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, utils }:
    utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };
        inherit (pkgs)
          dotnet-sdk
          emacs-nox
          emacsPackagesFor
          gnumake
          stdenv
          mkShell
          ;

        customEmacs = (emacsPackagesFor emacs-nox).emacsWithPackages
          (epkgs: with epkgs.melpaPackages; [
            citeproc
            ox-hugo
            ox-rss
          ]
          ++ (with epkgs.elpaPackages; [
            org
          ]));
      in
      {
        packages.default = stdenv.mkDerivation {
          name = "blog";
          src = ./.;
          buildInputs = [ customEmacs dotnet-sdk ];

          buildPhase = ''
            make publish
          '';

          installPhase = ''
            mkdir -p $out
            cp -r public/* $out
          '';
        };

        devShells.default = mkShell {
          buildInputs = [
            customEmacs
            gnumake
            dotnet-sdk
          ];
        };
      });
}
