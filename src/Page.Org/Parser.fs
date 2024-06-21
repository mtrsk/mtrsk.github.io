namespace Page.Org

open System


[<RequireQualifiedAccess>]
module Combinators =
    open FParsec
    
    type Item =
        | Keyword of Keyword
        | Block of string

    let manyContained popen pclose psep p = between popen pclose <| sepBy p psep
        
    // Applies popen, then pchar repeatedly until pclose succeeds,
    // returns the string in the middle.
    let manyCharsBetween popen pclose pchar = popen >>? manyCharsTill pchar pclose

    // Parses any string between popen and pclose
    let anyStringBetween popen pclose = manyCharsBetween popen pclose anyChar
    
    let endsWith (c: Char) = nextCharSatisfies (fun s -> s = c) <|> eof
    
    let removeSpaces (p: Parser<'a, 'b>): Parser<'a, 'b> =
        spaces
        >>. p
        .>> spaces

    // Parse all the data with the shape
    // #+<KEY>:<VALUE>
    let keyword =
        anyStringBetween (pstring "#+") (pchar ':')
        .>> spaces
        .>>. restOfLine true
        |>> Keyword

    // Consume as many keywords as you can
    let keywords = manyTill keyword newline
    
    // Parse Blocks
    let block =
        removeSpaces (restOfLine true)
        |>> Item.Block

    let blocks = manyTill block (endsWith '*')
        
    // Parse Sections
    let groupAsterisks = many (pchar '*')

    let section =
        removeSpaces groupAsterisks
        .>>. restOfLine true
        .>>. choice [
            keywords
            blocks
        ]
 
    let sections = manyTill section eof

    let parser =
        attempt keywords
        .>>. attempt blocks
        .>>. sections

    let execute (content: string) =
        run parser content
