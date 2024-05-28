target := env_var_or_default("ENVIRONMENT", "DEVELOPMENT")

build:
    dotnet fornax build

watch:
    dotnet fornax watch

clean:
    dotnet fornax clean
