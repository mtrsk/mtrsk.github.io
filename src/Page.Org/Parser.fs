namespace Page.Org

open System


//type Keywords =
//    { Title: string
//      Author: string
//      Date: DateTime option }
//    static member fromParser (input: (string * string) * string) =
//        let (title, author), time = input
//        let status, date =
//            time
//            |> (_.Substring(1, 10))
//            |> DateTime.TryParse
//        { Title = title
//          Author = author
//          Date = if status then Some date else None }
//
//type Section =
//    { Level: uint
//      Title: string
//      Content: string option }
//    static member fromParser (input: (char list * string) * string list) =
//        let (level, title), text = input
//        let content =
//            match text with
//            | [] -> None
//            | l -> String.concat "\n" l |> Some
//        { Level = level |> List.length |> uint
//          Title = title
//          Content = content }
 
[<RequireQualifiedAccess>]
module Combinators =
    open FParsec
    
    let manyContained popen pclose psep p = between popen pclose <| sepBy p psep
        
    // Applies popen, then pchar repeatedly until pclose succeeds,
    // returns the string in the middle.
    let manyCharsBetween popen pclose pchar = popen >>? manyCharsTill pchar pclose

    // Parses any string between popen and pclose
    let anyStringBetween popen pclose = manyCharsBetween popen pclose anyChar
    
    let endsWith (c: Char) = nextCharSatisfies (fun s -> s = c) <|> eof

    // Parse all the data with the shape
    // #+<KEY>:<VALUE>
    let keyword =
        anyStringBetween (pstring "#+") (pchar ':')
        .>>? skipChar ' '
        .>>. restOfLine true
        |>> Keyword
        
    let keywords = manyTill keyword newline
    
    // Parse Blocks
    let block =
        spaces
        >>. restOfLine true
        .>> spaces
        
    let blocks = manyTill block (endsWith '*')
        
    // Parse Sections
    let section =
        spaces
        >>. many (pchar '*')
        .>> spaces
        .>>. restOfLine true
        .>>. blocks
 
    let sections = manyTill section eof

    let parser =
        attempt keywords
        .>>. attempt blocks
        .>>. sections

    let execute (content: string) =
        run parser content
