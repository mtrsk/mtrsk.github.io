#r "nuget: FsToolkit.ErrorHandling, 4.15.2"

open System
open System.IO
open System.Text.RegularExpressions

open FsToolkit.ErrorHandling

type FilePath = FilePath of string
type Note =
    { Id: Guid
      Title: string
      Path: FilePath 
      References: Guid Set }
type ScriptErrors =
    | FileNotFound of Path: string
    | RegexMismatch of Name: string
    | InvalidGuid of Id: string

// Gets the solution's absolute path, using this script as basis
let root =
    __SOURCE_DIRECTORY__
    |> Path.GetFullPath
    |> string
let markdownPath = Path.Combine(root, "content")
let markdownNotesPath = Path.Combine(markdownPath, "notes")

// Handy regexes
let idMatch = @":ID:\s*([a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12})"
let idRx = Regex(idMatch, RegexOptions.Compiled)
let brokenLinkMatch = @"BROKEN LINK:\s*([a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12})"
let brokenLinkRx = Regex(brokenLinkMatch, RegexOptions.Compiled)
let markdownTitle = @"title\s*=\s*.(.*)."
let titleRx = Regex(markdownTitle, RegexOptions.Compiled)

let matchSingle (regex: Regex) content =
    let m = regex.Match(content)
    if m.Success then Ok m.Groups
    else RegexMismatch (regex.ToString()) |> Error

let matchMany (regex: Regex) content =
    regex.Matches(content)
    |> Seq.filter (fun m -> m.Success)
    |> Seq.map (fun m -> m.Groups)
    |> List.ofSeq

let readNote (path: FilePath) =
    result {
        match path with
        | FilePath p ->
            let content = File.ReadAllLines(p) |> String.concat "\n"
            let! title = matchSingle titleRx content |> Result.map (fun r -> r.[1].Value)
            let! noteId = matchSingle idRx content |> Result.map (fun r -> r.[1].Value |> Guid)
            let refs =
                content
                |> matchMany brokenLinkRx
                |> List.map (fun g -> g.[1].Value |> Guid)
                |> Set.ofList
            return { Id = noteId; Title = title; References = refs; Path = path }
    }

printfn "Reading: %A" markdownNotesPath

let notes =
    Directory.GetFiles(markdownNotesPath, "*.md")
    |> Array.map (fun p -> readNote (FilePath p))

printfn "%A" notes
