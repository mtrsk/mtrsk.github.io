;;; publish.el --- used to publish my org-roam notes with ox-hugo

;; Dependencies
(require 'ox)
(require 'ox-hugo)
(require 'org)
(require 'oc-csl)
(require 'citeproc)
(require 'find-lisp)

;;; Code:

;; Show debug information as soon as error occurs.
(toggle-debug-on-error)
;; Disable "<file>~" backups.
(setq make-backup-files nil)

;; Should be set up by the nix flake
(defconst project-root-dir
  (let* ((env-key "PROJECT_ROOT_DIR")
         (env-value (getenv env-key)))
    (if (and env-value (file-directory-p env-value))
        env-value
      (error (format "%s is not set or is not an existing directory (%s)" env-key env-value)))))

;; Configuration Variables:
(setq
 content-dir (concat (getenv "PWD") "/content-org")
 ;; Don't ask for confirmation before evaluating a code block
 org-confirm-babel-evaluate nil
 org-id-locations-file ".orgids"
 org-src-preserve-indentation t
 ;; org-cite settings
 org-cite-export-processors '((latex biblatex)
                              (moderncv basic)
                              (html csl)
                              (t csl))
 ;; Org Export settings
 ;; Broken links will be patched later in another script
 org-export-with-broken-links t
 org-export-with-toc nil
 org-export-with-smart-quotes t
 ;;org-export-with-todo-keywords nil
 ;; Only a single org file is expected here
 org-hugo-base-dir (concat (getenv "PWD") "/content-org/")
 ;; Org-Roam Settings
 org-roam-directory (concat (getenv "PWD") "/notes")
 org-roam-graph-export-directory (concat (getenv "PWD") "/content-org")
 org-id-extra-files (directory-files-recursively content-dir "\.org$"))

;; Functions

(defun build/export-file (&optional org-files-root-dir)
  "Exports a single Org files."
  ((message (format "[build] Looking for files at %s" org-files-root-dir))
   let (search-path (file-name-as-directory (expand-file-name org-files-root-dir)))
   (message (format "[build] Setting Search Path at %s" search-path))
   (dolist (org-file (directory-files-recursively search-path "\.org$"))
     (with-current-buffer (find-file org-file)
	   (message (format "[build] Exporting %s" org-file))
	   (org-hugo-export-wim-to-md :all-subtrees nil nil nil)))
   (message "Done!")))

(message (format "Setting CONTENT-DIR as %s" content-dir))
(build/export-all content-dir)

;;; publish.el ends here
