# Define source directory
BLOG_SRC := 'content-org'
BLOG_BUNDLE := 'site.tar.gz'

# Define source files using wildcard
SOURCES := `echo {{ BLOG_SRC }}/*.org`

default:
  just --list

# Generate Markdown files
content:
    emacs $(pwd) --batch --load export.el

# Publish content
public: content
    hugo

# Run Hugo server with drafts
run:
	hugo server --buildDrafts --buildFuture

# Build for CI
ci-build: clean public

# Removes org backups
remove-org:
    #!/usr/bin/env bash
    find . -iname "#*.org#" | xargs rm -r 

# Cleans the current environment
clean: remove-org
	rm -rf content
	rm -rf public
	rm -rf {{ BLOG_BUNDLE }}
