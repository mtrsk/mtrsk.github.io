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
        
let createOutputDirectory (p: Page.Views.Paths) =
    if Directory.Exists p.Outputs.Root then ()
    else Directory.CreateDirectory(p.Outputs.Root) |> ignore
    for dir in p.Outputs.Directories do
        let mutable path = Path.Combine(p.Outputs.Root, dir)
        if Directory.Exists path then ()
        else Directory.CreateDirectory(path) |> ignore
    done
    
    let assetsPath = Path.Combine(p.Outputs.Root, "assets")
    if Directory.Exists assetsPath then
        Directory.Delete(assetsPath, true)
    else
        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(p.Assets, assetsPath)

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
                  { Root = Path.Combine(solutionRoot, "public")
                    Directories = [ "notes"; "posts" ] } } }
 
    createOutputDirectory data.Paths
    let indexFileName = System.IO.Path.Combine(data.Paths.Outputs.Root, "index.html")
    let indexHtml = (Index.index data) |> RenderView.AsString.htmlDocument
    //File.WriteAllText(indexFileName, indexHtml)
    
    let orgFile = System.IO.Path.Combine(solutionRoot, "test.org")
    let htmlOutput = System.IO.Path.Combine(solutionRoot, "test.html")
    let content = File.ReadAllText(orgFile)
    Page.Org.Combinators.execute content
    |> printfn "%A"
    
    //let orgFile = System.IO.Path.Combine(solutionRoot, "test.org")
    //let htmlOutput = System.IO.Path.Combine(solutionRoot, "test.html")
    //let content = File.ReadAllText(orgFile)
    0
