ASSETS_DIR := static
ENVIRONMENT ?= "production"
FA_VERSION := 5.15.4
FA_URL := "https://use.fontawesome.com/releases/v$(FA_VERSION)/fontawesome-free-$(FA_VERSION)-web.zip"
MATHJAX_URL := "https://github.com/mathjax/MathJax.git"

# Include extra environment variables, mostly for development purposes
include .env
export

setup-fa:
	@curl $(FA_URL) --output fa.zip
	@mkdir -p ./$(ASSETS_DIR)/fontawesome
	@unzip -d ./$(ASSETS_DIR)/fontawesome fa.zip
	@mv ./$(ASSETS_DIR)/fontawesome/fontawesome-*/* ./$(ASSETS_DIR)/fontawesome/
	@rmdir ./$(ASSETS_DIR)/fontawesome/fontawesome-*/
	@echo "Removing unneeded files..."
	@rm fa.zip

setup-mathjax:
	@git clone $(MATHJAX_URL) mathjax
	@mv ./mathjax $(ASSETS_DIR)
	@cd ./$(ASSETS_DIR)/mathjax
	@npm install
	@npm run compile
	@npm run make-components

publish: publish.el
	@echo "ENVIRONMENT=$(ENVIRONMENT)"
	@([[ -d ./public ]] && rm -rf ./public) || echo "Skipping directory creation..."
	@([[ -d $$HOME/.org-timestamps ]] && rm -rf $$HOME/.org-timestamps) || echo "Skipping.."
	@echo "Publishing... with current Emacs configurations."
	ENVIRONMENT=$(ENVIRONMENT) \
		emacs --batch --load publish.el --funcall org-publish-all
	@cp -r css ./public
	@cp -r static ./public
	@cp ./images/nixos.gif ./public/images
