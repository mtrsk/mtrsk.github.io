namespace Page.Views

open System
open System.IO

open Giraffe.ViewEngine

type Page =
    { Head: Head }
and Head =
    { Title: string
      Metadata: Metadata list }
and Metadata = { Name: string; Content: string }

[<RequireQualifiedAccess>]
module Index =
    let private addMetadata (metadata: Metadata list) =
        metadata
        |> List.map (fun m ->
            meta [ _name m.Name
                   _content m.Content ]
        )
        
    let jsLibraries =
        let applicationType = "application/javascript"
        let highlightJs version =
            [ link [ _rel "stylesheet"
                     _href $"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/${version}/styles/default.min.css" ]
              script [ _type applicationType
                       _src $"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/{version}/highlight.min.js" ] []
              // Extra languages
              script [ _type applicationType
                       _src $"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/{version}/languages/nix.min.js" ] []
              script [ _type applicationType
                       _src $"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/{version}/languages/fsharp.min.js" ] []
              script [ _type applicationType ] [ rawText "hljs.highlightAll();" ] ]
        let mathJax =
            [ script [ _src "https://polyfill.io/v3/polyfill.min.js?features=es6"
                       _type applicationType ] []
              script [ _src "https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js"
                       _type applicationType
                       _id "MathJax-script"
                       _async ] [] ]
        let boxIcons version =
            [ link [ _rel "stylesheet"
                     _href $"https://unpkg.com/boxicons@{version}/css/boxicons.min.css" ] ]
        (highlightJs "11.9.0") @ mathJax @ (boxIcons "2.1.4")
    
    let headView (page: Page) =
        let metadata = addMetadata page.Head.Metadata
        head [] ([
            title [] [ str page.Head.Title ]
        ] @ metadata @ jsLibraries)
        
    let menu (page: Page) =
        [ div [] [
              header [ _class "header" ] [
                  a [ _href "./index.html" ] [ str "Home" ]
                  nav [] [
                      a [ _href "./notes.html" ] [ str "Notes" ]
                      a [ _href "./posts.html" ] [ str "Posts" ]
                  ]
                  h1 [ _class "title" ] [ str page.Head.Title ]
              ]
          ]
        ]
    
    let profiles =
        [ p [] [ str "You may also find me elsewhere in cyberspace, under the following aliases:" ]
          div [ _class "flex-container" ] [
            div [ _class "flex-item" ] [
                a [ _href "https://github.com/mtrsk"
                    _title "My Github profile" ] [ i [ _class "bx bxl-github bx-lg" ] [] ]
            ]
            
            div [ _class "flex-item" ] [
                a [ _href "https://gitlab.com/mtrsk"
                    _title "My Gitlab profile" ] [ i [ _class "bx bxl-gitlab bx-lg" ] [] ]
            ]
            
            div [ _class "flex-item" ] [
                a [ _href "https://www.linkedin.com/in/schonfinkel"
                    _title "My Linkedin profile" ] [ i [ _class "bx bxl-linkedin-square bx-lg" ] []]
            ]
            
            div [ _class "flex-item" ] [
                a [ _href "mailto:marcos.schonfinkel@gmail.com"
                    _title "My Email" ] [ i [ _class "bx bx-envelope bx-lg" ] []]
            ]
          ]
        ]

    let aboutView =
        let interests =
            [ "Backend Development"
              "Infrastructure"
              "Functional Programming"
              "GNU/Linux &amp; FOSS"
              "Logic"
              "Philosophy" ]
            |> List.map (fun i ->
                li [] [ Text i ]
            )
        [ h2 [] [ str "About" ]
          p [] [ Text "Hello! I'm <b>Marcos Benevides</b>, a developer from São Luís, MA - Brazil. You're going to find my posts, projects and tutorials here, and probably some random rambling about byzantine history." ]
          h3 [] [ str "Interests" ]
          p [] [ str "Here's a non-exhaustive list of my current interests:" ]
          ul [] interests
          figure [] [
              img [ _src "./assets/nixos.gif"
                    _alt "nixos.gif"
                    _width "25%"
                    _height "25%" ]
              figcaption [] [
                Text "btw, I use <a href=\"https://nixos.org/\">NixOS</a>&#x2026;"
              ]
          ] ] @ profiles

    let footerView =
        let template (updated: DateTimeOffset) =
            let format = updated.ToString("o")
            let repo = "https://github.com/mtrsk/mtrsk.github.io"
            let source src = $"<a href=\"{src}\">here</a>"
            let builtWith =
                String.Join (", ", [|
                    "<a href=\"https://fsharp.org/\">F#</a>"
                    "<a href=\"https://github.com/mtrsk/craft\">Craft</a>"
                    "<a href=\"https://orgmode.org/\">Orgmode</a>"
                |]) + " and <a href=\"https://nixos.org/\">Nix</a>"
            $"Last updated at {format}.<br> Built with {builtWith}. Source code available {source repo}."
        footer [] [
            p [] [
                Text (template DateTimeOffset.Now)
            ]
        ]

    let indexBodyView (page: Page) =
        body [] (menu page @ aboutView @ [ footerView ])
    
    let index (page: Page) =
        html [] [
            headView page
            indexBodyView page
        ]
