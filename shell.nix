{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  name = "dev-shell";
  buildInputs = with pkgs; [
    dotnet-sdk_5
    # keep this line if you use bash
    pkgs.bashInteractive
  ];
}

