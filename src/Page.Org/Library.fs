namespace Page.Org

open System

type Exporters =
    { Title: string
      Author: string
      Date: DateTime option }

type Section =
    { Level: uint
      Title: string
      Content: string option }
    static member fromParser (input: (char list * string) * string list) =
        let (level, title), text = input
        let content =
            match text with
            | [] -> None
            | l -> String.concat "\n" l |> Some
        { Level = level |> List.length |> uint
          Title = title
          Content = content }


[<RequireQualifiedAccess>]
module Combinators =
    open FParsec
    
    let exporter (field: string) =
        skipString "#+"
        .>> (pstring (field.ToUpper()) <|> pstring (field.ToLower()))
        .>> skipChar ':'
        .>>? skipChar ' '
        >>. restOfLine true
 
    let exporters =
        parse {
            let! title = exporter "title"
            let! author = exporter "author"
            let! status, date =
                exporter "date"
                |>> (_.Substring(1, 10))
                |>> DateTime.TryParse
            return { Title = title; Author = author; Date = if status then Some date else None }
        }
        
    let sectionContent =
        manyTill (spaces >>. restOfLine true .>> spaces) (nextCharSatisfies (fun c -> c = '*') <|> eof)
        
    let section =
        many (pchar '*')
        .>> spaces
        .>>. restOfLine true
        .>>. sectionContent
        |>> Section.fromParser
    
    let sections = manyTill section eof
    
    let manyContained popen pclose psep p = between popen pclose <| sepBy p psep
        
    let parser =
        exporters
        .>> spaces
        .>>. sections

    let execute (content: string) =
        run parser content
