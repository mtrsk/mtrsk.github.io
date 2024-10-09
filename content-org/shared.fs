module Shared

open System
open System.IO

let private projectRoot = __SOURCE_DIRECTORY__ |> string |> Directory.GetParent

let private blogDirectory = Path.Join(projectRoot.FullName, "blog")
let private postsDirectory = Path.Join(blogDirectory, "posts")
let private notesDirectory = Path.Join(projectRoot.FullName, "notes")
let private markdownDirectory = Path.Join(projectRoot.FullName, "content")
let private markdownNotesDirectory = Path.Join(markdownDirectory, "notes")

let private noteCreatedAt (s: String) = s.[0..13]
let private postCreatedAt (s: String) = s.[0..7]

let posts =
    Directory.EnumerateFiles(postsDirectory)
    |> Seq.map System.IO.Path.GetFileName
    |> List.ofSeq
    |> List.filter (fun s -> s.StartsWith(".") |> not)
    |> List.sortByDescending (postCreatedAt)

let notes =
    Directory.EnumerateFiles(notesDirectory)
    |> Seq.map System.IO.Path.GetFileName
    |> List.ofSeq
    |> List.filter (fun s -> s.StartsWith(".") |> not)
    |> List.sortByDescending (noteCreatedAt)

let markdownNotes =
    Directory.EnumerateFiles(markdownNotesDirectory)
    |> Seq.map System.IO.Path.GetFileName
    |> List.ofSeq
    |> List.filter (fun s -> s.StartsWith(".") |> not)

let capitalize (s: String) =
    s.Split([| '_' |])
    |> Array.map (fun w -> (Char.ToUpper(w.[0]) |> string) + w.[1..])
    |> String.concat (" ")
