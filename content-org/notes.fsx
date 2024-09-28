open System
open System.Collections
open System.IO

let projectRoot = __SOURCE_DIRECTORY__ |> string |> Directory.GetParent

let blogDirectory = Path.Join(projectRoot.FullName, "blog")
let notesDirectory = Path.Join(projectRoot.FullName, "notes")

let createPost () =
    $"""

    """
