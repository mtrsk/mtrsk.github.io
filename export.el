;;; export --- used to export my org-roam notes with ox-hugo

(require 'ox)
(require 'org)
(require 'org-publish)
(require 'oc-csl)
(require 'citeproc)
(require 'find-lisp)

;; Configuration Variables:
(setq content-dir (concat (getenv "PWD") "/content-org"))
(setq org-roam-directory (concat (getenv "PWD") "/notes"))
(setq org-roam-graph-export-directory (concat (getenv "PWD") "/content-org"))
(setq org-blog-directory (concat (getenv "PWD") "/blog"))
(setq org-element-use-cache nil)

;; https://jeffkreeftmeijer.com/org-unable-to-resolve-link/
;; (org-id-update-id-locations (directory-files-recursively org-roam-directory "\\.org$") )

;; Don't ask for confirmation before evaluating a code block
(setq org-confirm-babel-evaluate nil)
;;(setq org-export-use-babel t)
(setq org-export-with-broken-links t)

(setq org-roam-graph-link-builder 'org-roam-custom-link-builder)

;; Functions
;; Regular posts
(setq org-id-locations-file ".orgids"
      org-src-preserve-indentation t
      org-cite-export-processors '((latex biblatex)
                                   (moderncv basic)
                                   (html csl)
                                   (t csl)))

(ox-hugo/export-all content-dir t)

;;; export.el ends here
