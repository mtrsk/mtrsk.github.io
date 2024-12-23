# Define source directory
BLOG_SRC := 'content-org'
BLOG_BUNDLE := 'site.tar.gz'
STATIC_SRC := 'static'
STATIC_JS_SRC := STATIC_SRC / 'js'

D3_VERSION := 'v7'
D3_FILENAME := "d3.min.js"
D3_URL := "https://d3js.org/d3." + D3_VERSION + ".min.js"
D3_PATH := STATIC_JS_SRC / D3_FILENAME

# Define source files using wildcard
SOURCES := `echo {{ BLOG_SRC }}/*.org`

default:
  just --list

# Generate the Markdown files
build: clean
    emacs $(pwd) --batch --load export.el
    python fix-ref-links.py

# Publish content
public: build
    hugo

# Download D3
d3-download:
    curl -L -o {{ D3_PATH }} {{ D3_URL }}

# Build braph
graph:
    python generate-graph.py

# Run Hugo server with drafts
run: build
	hugo server --buildDrafts --buildFuture --disableFastRender

# Build for CI
ci-build: clean
    nix develop .#ci --impure -c just build

ci-publish: ci-build
    nix develop .#ci --impure -c hugo

ci-run:
    nix develop .#ci --impure -c just run

# Removes org backups
remove-org:
    #!/usr/bin/env bash
    find . -iname "#*.org#" | xargs rm -f
    find . -iname "*~undo-tree~" | xargs rm -f
    echo "Finished!"

# Cleans the current environment
clean: remove-org
    rm -rf content
    rm -f {{ BLOG_SRC }}/.#content.org
    rm -rf public
    rm -rf {{ BLOG_BUNDLE }}
