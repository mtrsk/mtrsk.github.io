open System
open System.IO
open System.Text.RegularExpressions

type Post = { Path: string; Title: string }

let root =
    __SOURCE_DIRECTORY__
    |> Path.GetFullPath
    |> string
let output = "index"

let rxTitle = Regex(@"\+[Tt][Ii][Tt][Ll][Ee]:\s?(.+)", RegexOptions.Compiled)

let readFile(path: string) =
    let text =
        File.ReadAllLines(path)
        |> String.concat "\n"
    (path, text)

let check (regex: Regex) expr =
    let m = regex.Match(expr)
    if m.Success then Some (m.Value, m.Groups[1])
    else None

let parse path (content: string) =
    let group = check rxTitle content
    match group with
    | Some (_, t) ->
        Some { Title = t.Value; Path = path; }
    | _ -> None

let generateHtml (posts: Post []) filename outpath =
    task {
        let render (p: Post) =
            let path =
                p.Path.Replace(root, ".").Replace(".org", ".html")
            $"<li><a href={path}>{p.Title}</a></li>"
        let list =
            posts
            |> Array.sortByDescending (fun p -> p.Path)
            |> Array.map render
            |> String.concat "\n"

        let orgTemplate = $"
            #+begin_export html
                {list}
            #+end_export
        "
        let path = $"{outpath}/{filename}.org"
        printfn "%A" path
        use file = File.Create(path)
        let bytes = System.Text.Encoding.UTF8.GetBytes(orgTemplate)
        do! file.WriteAsync (ReadOnlyMemory bytes)
    }

let posts =
    Directory.GetFiles(root, "*.org")
    |> Array.map readFile
    |> Array.choose (fun (p,c) -> parse p c)

generateHtml posts output root
|> Async.AwaitTask
|> Async.RunSynchronously
