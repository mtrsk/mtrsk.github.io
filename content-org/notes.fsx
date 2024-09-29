#load "shared.fs"

open System

let createNote (fileName: String) =
    let createdAt = fileName.[0..13]
    let date = $"{createdAt.[0..3]}-{createdAt.[4..5]}-{createdAt.[6..7]}"
    let name = fileName.[15..].Replace(".org", "")
    let slug = name.Replace("_", "-")
    let title = Shared.capitalize name

    $"
** {title}
:PROPERTIES:
:EXPORT_FILE_NAME: {slug}
:EXPORT_DATE: {date}
:EXPORT_HUGO_CUSTOM_FRONT_MATTER: :slug {slug}
:CUSTOM_ID: {slug}
:END:

#+INCLUDE: \"../notes/{fileName}\"
    "

let org = Shared.notes |> List.map (createNote) |> (fun s -> String.Join("\n", s))
printfn "%s" org
