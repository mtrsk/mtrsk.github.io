namespace Page.Org

open System

type Keywords =
    { Title: string
      Author: string
      Date: DateTime option }
    static member fromParser (input: (string * string) * string) =
        let (title, author), time = input
        let status, date =
            time
            |> (_.Substring(1, 10))
            |> DateTime.TryParse
        { Title = title
          Author = author
          Date = if status then Some date else None }

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
    
    let keyword (field: string) =
        skipString "#+"
        .>> (pstring (field.ToUpper()) <|> pstring (field.ToLower()))
        .>> skipChar ':'
        .>>? skipChar ' '
        >>. restOfLine true
 
    let keywords =
        keyword "title"
        .>>. keyword "author"
        .>>. keyword "date"
        |>> Keywords.fromParser
        
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
        keywords
        .>> spaces
        .>>. sections

    let execute (content: string) =
        run parser content
