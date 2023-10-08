open System
open System.IO
open System.Text.RegularExpressions

type Post = { Path: string; Title: string; Date: DateTime }

let root =
    __SOURCE_DIRECTORY__
    |> Path.GetFullPath
    |> string
let path = Path.Combine(root, "blog/posts")
let output = Path.Combine(root, "blog")

let rxTitle = Regex(@"\+TITLE:\s?(.+)", RegexOptions.Compiled)
let rxDate = Regex(@"\+DATE:\s?<(\d{4}-\d{2}-\d{2}).+>", RegexOptions.Compiled)

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
    let group1 = check rxTitle content
    let group2 = check rxDate content
    match (group1, group2) with
    | Some (_, t), Some (_, d) ->
        let isOk, date = DateTime.TryParse(d.Value)
        if isOk then
            Some { Title = t.Value; Date = date; Path = path; }
        else None
    | _ -> None

let generateHtml (posts: Post []) =
    let render (p: Post) =
        let path =
            p.Path.Replace(root + "/blog", ".").Replace(".org", ".html")
        $"<li><a href={path}>{p.Title}</a></li>"
    let list =
        posts
        |> Array.sortByDescending (fun p -> p.Date)
        |> Array.map render
        |> String.concat "\n"

    let orgTemplate = $"
        #+begin_export html
        {list}
        #+end_export
    "
    use writer = new StreamWriter(path = output + "/posts.org")
    writer.WriteLine(orgTemplate)

let posts =
    Directory.GetFiles(path, "*.org")
    |> Array.map readFile
    |> Array.choose (fun (p,c) -> parse p c)

generateHtml posts