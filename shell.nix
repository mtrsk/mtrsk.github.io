{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  name = "dev-shell";
  buildInputs = with pkgs; [
    dotnet-sdk_3
    nodejs-14_x
  ];
}

