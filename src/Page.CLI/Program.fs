open System
open System.IO

open Giraffe.ViewEngine

open Page.Views

let solutionRoot = __SOURCE_DIRECTORY__ |> string |> Directory.GetParent |> string
let assetsPath = Path.Combine(solutionRoot, "assets")
let imagesPath = Path.Combine(assetsPath, "images")
let stylesPath = Path.Combine(assetsPath, "css")

[<EntryPoint>]
let main args =
    let fileName = System.IO.Path.Combine(solutionRoot, "index.html")
    let data: Page =
        { Head =
            { Title = "Stranger in a strange λ"
              Metadata =
                [ { Name = "author"; Content = "Marcos Benevides" } ] } }
    let html = (Index.index data) |> RenderView.AsString.htmlDocument
    System.IO.File.WriteAllText(fileName, html)
    
    let orgFile = System.IO.Path.Combine(solutionRoot, "test.org")
    let htmlOutput = System.IO.Path.Combine(solutionRoot, "test.html")
    
    let content = File.ReadAllText(orgFile)
    0
