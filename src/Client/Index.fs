module Index

open Elmish
open Fable.Remoting.Client
open Fable.FontAwesome
open Fulma
open Shared

type Model =
    { Todos: Todo list
      Input: string }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo

let todosApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let init(): Model * Cmd<Msg> =
    let model =
        { Todos = []
          Input = "" }
    let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotTodos todos ->
        { model with Todos = todos }, Cmd.none
    | SetInput value ->
        { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input
        let cmd = Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo
        { model with Input = "" }, cmd
    | AddedTodo todo ->
        { model with Todos = model.Todos @ [ todo ] }, Cmd.none

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]


let createNavBar =
    Navbar.navbar [ Navbar.Color IsLight ]
        [ Navbar.Brand.div [  ]
            [ Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                [ img [ Style [ Width "2.5em" ] // Force svg display
                        Src "assets/turnstile.svg" ] ] ]
          Navbar.Item.a [ Navbar.Item.HasDropdown
                          Navbar.Item.IsHoverable ]
            [ Navbar.Link.a [ ]
                [ str "Language" ]
              Navbar.Dropdown.div [ ]
                [ Navbar.Item.a [ ]
                    [ str "English" ]
                ]
            ]

          Navbar.Item.a [ Navbar.Item.IsHoverable ]
            [ Navbar.Link.a [ Navbar.Link.IsArrowless ] [ str "Posts" ]
            ]

          Navbar.Item.a [ Navbar.Item.IsHoverable ]
            [ Navbar.Link.a [ Navbar.Link.IsArrowless ] [ str "Notes" ]
            ]

          Navbar.End.div [ ]
            [ Navbar.Item.a [
                Navbar.Item.Props [ Href "https://github.com/mtrsk" ]
              ] [
                Icon.icon [ ] [ Fa.i [ Fa.Brand.Github ] [ ] ]
              ]

              Navbar.Item.a [
                Navbar.Item.Props [ Href "https://www.linkedin.com/in/marcos-schonfinkel/" ]
              ] [
                Icon.icon [ ] [ Fa.i [ Fa.Brand.Linkedin ] [ ] ]
              ]

              Navbar.Item.a [
                Navbar.Item.Props [ Href "https://stackexchange.com/users/5858235/aristu" ]
              ] [
                Icon.icon [ ] [ Fa.i [ Fa.Brand.StackOverflow ] [ ] ]
              ]
            ]
        ]


let buildView =
    Hero.hero [
        Hero.Color IsLight
        Hero.IsFullHeight
        //Hero.Props [
        //    Style [
        //        Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
        //        BackgroundSize "cover"
        //    ]
        //]
    ] [
        Hero.head [ ] [
          createNavBar
        ]

        Hero.body [ ] [
          Container.container [
            Container.IsFluid
            Container.Modifiers [
              Modifier.TextAlignment (Screen.All, TextAlignment.Centered)
              Modifier.BackgroundColor IsGreyLighter
            ]
          ] [
            Heading.h3 [ ] [ str "My Name" ]
            Heading.h4 [ Heading.IsSubtitle ] [ str "Software Developer" ]
          ]
        ]
    ]


let buildFooter =
  Footer.footer [ Modifiers [ Modifier.BackgroundColor IsLight ] ]
    [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
    [ p [ ] [ str "Proudly built with F# and the SAFE Stack. " ] ] ]


let view (model : Model) (dispatch : Msg -> unit) =
    Container.container [
      Container.IsFluid
      Container.Modifiers [
        Modifier.TextAlignment (Screen.All, TextAlignment.Centered)
        Modifier.BackgroundColor IsLight
        Modifier.Spacing (Spacing.PaddingLeftAndRight, Spacing.Is6)
      ]
    ] [
      buildView
      buildFooter
    ]
