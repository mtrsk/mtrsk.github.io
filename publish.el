;; package --- Summary:
;; org-publish.el --- publish related org-mode files as a website
;;; Commentary:
;;;
;;; How to run this:
;;; * eval the buffer
;;; * M-x site-publish
;;;

(require 'citeproc)
(require 'find-lisp)
(require 'oc-csl)
(require 'org)
(require 'ox)
(require 'ox-html)
(require 'ox-publish)

;;; org-babel configuration

;;; Functions:
;;;; http://xahlee.info/emacs/emacs/elisp_read_file_content.html
(defun get-string-from-file (filePath)
  "Return file content as string from FILEPATH."
  (with-temp-buffer
    (insert-file-contents filePath)
    (buffer-string)))

;;; Project variables:
;;;; don't ask for confirmation before evaluating a code block
(setq org-confirm-babel-evaluate nil)
(setq org-export-use-babel t)
(setq org-export-with-broken-links "mark")
(setq org-element-use-cache nil)

;;;; Settings
(setq-default root-dir (if (string= (getenv "ENVIRONMENT") "dev") (getenv "PWD") ""))
(setq static-dir (concat root-dir "/static"))
(setq org-blog-dir (concat root-dir "/blog"))
(setq org-roam-dir (concat root-dir "/notes"))

(message (format "SETTING ROOT DIR: %s" root-dir))
(message (format "SETTING STATIC DIR: %s" static-dir))
(message (format "SETTING BLOG DIR: %s" static-dir))
(message (format "SETTING NOTES DIR: %s" static-dir))

(defcustom out-dir (format "%s/public" root-dir) "Directory where the HTML files are going to be exported.")
(message (format "SETTING OUT DIR: %s" out-dir))

;;;; Customize the HTML output
(setq-default website-html-head (concat "<script type=\"text/javascript\" src=\"https://d3js.org/d3.v7.min.js\"></script>\n"
                                        "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/d3/7.9.0/d3.min.js\"></script>\n"
                                        "<script id=\"MathJax-script\" async src=\"https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-chtml.js\"></script>\n"
                                        "<script>\n"
                                        "  MathJax = {\n"
                                        "    tex: {\n"
                                        "       displayMath: [['\\[', '\\]'], ['$$', '$$']],\n"
                                        "       inlineMath: [['\\(', '\\)']\n"
                                        "    };\n"
                                        "  };\n"
                                        "</script>\n"
                                        (format "<link href=\"%s/css/main.scss\" rel=\"stylesheet\">\n" out-dir)
                                        "<link href=\"https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css\" rel=\"stylesheet\">\n"))

(setq-default website-html-preamble (concat (format "<nav><img src=\"%s/img/logo.png\" alt=\"logo\" width=\"128\" height=\"128\" />\n" out-dir)
                                            "<div><h1>Benevides' Blog</h1>"
                                            "<p>\"Writing is nothing more than a guided dream\"</p></div>\n"
                                            "<ul>\n"
                                            "<li><a href='/'>Home</a></li>\n"
                                            "  <li><a href='/blog'>Blog</a></li>\n"
                                            "  <li><a href='/notes'>Notes</a></li>\n"
                                            "  <li><a href='/blogroll'>BlogRoll</a></li>\n"
                                            "</ul></nav>\n"))

(setq-default website-html-postamble (concat "<footer></br>\n"
                                             "<p>Built with <a href=https://orgmode.org/>Orgmode</a>, <a href=https://www.gnu.org/software/emacs/>Emacs</a> and <a href=https://nixos.org/>Nix</a>, source code availiable <a href=https://github.com/mtrsk/mtrsk.github.io>here</a>.</p>\n"
                                             "</footer>"))


;;; Code:

(setq org-publish-project-alist
      `(("blog"
         :base-directory ,root-dir
         :base-extension "org"
         :publishing-directory ,out-dir
         :publishing-function org-html-publish-to-html

         :author "Marcos Benevides"
         :email "(concat \"marcos.schonfinkel\" at \"gmail.com\")"
         :with-creator t
         :recursive t

         :export-with-tags nil
         :exclude-tags ("todo" "noexport")
         :exclude "level-.*\\|.*\.draft\.org"
         :with-title t
         :section-numbers nil
         :headline-levels 5
         :with-toc nil
         :with-date t

         :auto-sitemap nil
         :sitemap-filename "index.org"
         :sitemap-title "Home"
         :sitemap-sort-files anti-chronologically
         :sitemap-file-entry-format "%d - %t"
         :sitemap-style list

         :html-doctype "html5"
         :html-html5-fancy t
         :html-head ,website-html-head
         :html-preamble ,website-html-preamble
         :html-postamble ,website-html-postamble)

        ("static"
         :base-directory ,static-dir
         :base-extension "scss\\|css\\|js\\|png\\|jpg\\|gif\\|pdf\\|mp3\\|ogg\\|swf\\|json"
         :publishing-directory ,out-dir
         :recursive t
         :publishing-function org-publish-attachment)

        ("all" :components ("blog" "static"))))

;; Generate the site output
(org-publish-all t)

(message "Build complete!")

;;; publish.el ends here
