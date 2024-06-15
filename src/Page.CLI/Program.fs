open System
open System.IO

open Giraffe.ViewEngine

open Page.Views

let rec getParentDirectoryAt (path: String) (level: uint) =
    match level with
    | 0u -> path
    | n ->
        let parent = path |> Directory.GetParent |> string
        getParentDirectoryAt parent (n - 1u)

let solutionRoot = getParentDirectoryAt __SOURCE_DIRECTORY__ 2u

let assetsPath = Path.Combine(solutionRoot, "assets")
let imagesPath = Path.Combine(assetsPath, "images")
let stylesPath = Path.Combine(assetsPath, "css")

[<EntryPoint>]
let main args =
    let data: Page =
        { Head =
            { Title = "Stranger in a strange λ"
              Metadata =
                [ { Name = "author"; Content = "Marcos Benevides" } ] }
          Paths =
            { Assets = assetsPath
              Images = imagesPath
              Styles = stylesPath
              Outputs =
                  { Root = solutionRoot
                    Posts = "posts"
                    Notes = "notes" } } }
        
    let indexFileName = System.IO.Path.Combine(solutionRoot, "index.html")
    let indexHtml = (Index.index data) |> RenderView.AsString.htmlDocument
    File.WriteAllText(indexFileName, indexHtml)
    
    //let orgFile = System.IO.Path.Combine(solutionRoot, "test.org")
    //let htmlOutput = System.IO.Path.Combine(solutionRoot, "test.html")
    //let content = File.ReadAllText(orgFile)
    0
