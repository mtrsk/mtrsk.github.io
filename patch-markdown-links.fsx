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

let fileExists (path: FilePath) =
    match path with
    | FilePath p -> if Path.Exists p then Ok path else FileNotFound p |> Error

let toGuid (s: string) =
    let status, guid = Guid.TryParse s
    if status then Ok guid else InvalidGuid s |> Error

let readNote (path: FilePath) =
    result {
        match path with
        | FilePath p ->
            let content = File.ReadAllLines(p) |> String.concat "\n"
            let! title = 
                matchSingle titleRx content |> Result.map (fun r -> r.[1].Value)
            let! noteId =
                matchSingle idRx content |> Result.map (fun r -> r.[1].Value) |> Result.bind toGuid
            let! refs =
                content
                |> matchMany brokenLinkRx
                |> List.map (fun g -> g.[1].Value)
                |> List.traverseResultM toGuid
            return { Id = noteId; Title = title; References = refs |> Set.ofList; Path = path }
    }

let buildMap (r: Result<Note, ScriptErrors> array) =
    let rec loop (m: Map<Guid, Note>) i stop =
        if i = stop then m
        else
            match r[i] with
            | Ok n -> loop (Map.add n.Id n m) (i + 1) stop
            | Error e -> 
                printfn "%A" e
                loop m (i + 1) stop
    let l = Array.length r
    if l = 0 then Map.empty
    else loop Map.empty 0 (l - 1)

let patchMarkdown (notes: Map<Guid, Note>) (n: Note) =
    let refTitles = 
        n.References
        |> Set.map (fun k -> Map.find k notes)
    let ref =
        0
    ()

printfn "Reading: %A" markdownNotesPath

let notes =
    Directory.GetFiles(markdownNotesPath, "*.md")
    |> Array.Parallel.map (FilePath >> fileExists >> Result.bind readNote)
    |> buildMap

let withReferences = 
    notes 
    |> Map.filter (fun _ v -> Set.isEmpty v.References |> not)

printfn "%A" withReferences
