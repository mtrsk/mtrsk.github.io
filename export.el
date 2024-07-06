;;; export --- used to export my org-roam notes with ox-hugo

(require 'ox)
(require 'ox-hugo)
(require 'org)
(require 'oc-csl)
(require 'citeproc)

;; Configuration Variables:
(setq content-dir (concat (getenv "PWD") "/content-org"))

;; Don't ask for confirmation before evaluating a code block
;;(setq org-confirm-babel-evaluate nil)
;;(setq org-export-use-babel t)
(setq org-export-with-broken-links t)

;; This makes it easier to export my notes into something that
;; ox-hugo will easily pick links and generate markdowns.
;; https://www.orgroam.com/manual.html#Overriding-the-default-link-creation-function
;;(defun org-roam-custom-link-builder (node)
;;  (let ((file (org-roam-node-file node)))
;;    (concat (file-name-base file) ".html")))
;;
;;(setq org-roam-graph-link-builder 'org-roam-custom-link-builder)

;;(setq org-roam-graph-link-builder 'org-roam-custom-link-builder)

;; Functions

(defun ox-hugo/export-all (&optional org-files-root-dir dont-recurse)
  "Export all Org files (including nested) under ORG-FILES-ROOT-DIR.

All valid post subtrees in all Org files are exported using
`org-hugo-export-wim-to-md'.

If optional arg ORG-FILES-ROOT-DIR is nil, all Org files in
current buffer's directory are exported.

If optional arg DONT-RECURSE is nil, all Org files in
ORG-FILES-ROOT-DIR in all subdirectories are exported. Else, only
the Org files directly present in the current directory are
exported.  If this function is called interactively with
\\[universal-argument] prefix, DONT-RECURSE is set to non-nil.

Example usage in Emacs Lisp: (ox-hugo/export-all \"~/org\")."
  (interactive)
  (let* ((org-files-root-dir (or org-files-root-dir default-directory))
         (dont-recurse (or dont-recurse (and current-prefix-arg t)))
         (search-path (file-name-as-directory (expand-file-name org-files-root-dir)))
         (org-files (if dont-recurse
                        (directory-files search-path :full "\.org$")
                      (directory-files-recursively search-path "\.org$")))
         (num-files (length org-files))
         (cnt 1))
    (if (= 0 num-files)
        (message (format "No Org files found in %s" search-path))
      (progn
        (message (format (if dont-recurse
                             "[ox-hugo/export-all] Exporting %d files from %S .."
                           "[ox-hugo/export-all] Exporting %d files recursively from %S ..")
                         num-files search-path))
        (dolist (org-file org-files)
          (with-current-buffer (find-file-noselect org-file)
            (message (format "[ox-hugo/export-all file %d/%d] Exporting %s" cnt num-files org-file))
            (org-hugo-export-wim-to-md :all-subtrees)
            (setq cnt (1+ cnt))))
        (message "Done!")))))

;; Regular posts
(setq org-id-locations-file ".orgids"
      org-src-preserve-indentation t
      org-cite-export-processors '((latex biblatex)
                                   (moderncv basic)
                                   (html csl)
                                   (t csl)))

(ox-hugo/export-all content-dir t)

;;; export.el ends here
