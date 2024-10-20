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
 content-dir (concat project-root-dir "/content-org/")
 blog-dir (concat project-root-dir "/blog/posts/")
 notes-dir (concat project-root-dir "/notes/")
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

(defun publish (file)
  (with-current-buffer (find-file-noselect file)
    (let ((org-id-extra-files (find-lisp-find-files org-roam-directory "\.org$")))
      (org-hugo-export-wim-to-md))))

(defun get-org-files-in-directory (dir)
  "Print and return a list of all .org files in DIR and its subdirectories."
  (message "[build] Directory: %s" dir)
  (dolist (org-file (directory-files-recursively dir "\.org$"))
    (message (format "[build] Found: %s" org-file))
    (publish org-file))
  (message "Done!"))

                                        ; (defun build/export-file (org-file)
                                        ;   "Exports a single ORG-FILE as markdown."
                                        ;   ((message (format "[build] Looking for files at %s" org-files-root-dir))
                                        ;    let (search-path (file-name-as-directory (expand-file-name org-files-root-dir)))
                                        ;    (message (format "[build] Setting Search Path at %s" search-path))
                                        ;    (dolist (org-file (directory-files-recursively search-path "\.org$"))
                                        ;      (with-current-buffer (find-file org-file)
                                        ;        (message (format "[build] Exporting %s" org-file))
                                        ;        (org-hugo-export-wim-to-md :all-subtrees nil nil nil)))
                                        ;    (message "Done!")))

(message (format "Setting NOTES-DIR as %s" notes-dir))
;;(get-org-files-in-directory notes-dir)

(message (format "Setting BLOG-DIR as %s" blog-dir))
(get-org-files-in-directory blog-dir)

(message (format "Setting CONTENT-DIR as %s" content-dir))
(get-org-files-in-directory content-dir)
;;(build/export-file content-dir)

;;; publish.el ends here
