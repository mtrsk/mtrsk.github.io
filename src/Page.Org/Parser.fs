namespace Page.Org

open System


[<RequireQualifiedAccess>]
module Combinators =
    open FParsec
    
    type Item =
        | Keyword of Keyword
        | Block of Block 

    let manyContained popen pclose psep p = between popen pclose <| sepBy p psep
        
    // Applies popen, then pchar repeatedly until pclose succeeds,
    // returns the string in the middle.
    let manyCharsBetween popen pclose pchar = popen >>? manyCharsTill pchar pclose

    // Parses any string between popen and pclose
    let anyStringBetween popen pclose = manyCharsBetween popen pclose anyChar
    
    //let endsWith (c: Char) = nextCharSatisfies (fun s -> s = c) <|> eof
    let nextCharMatches (c: Char) = nextCharSatisfies (fun x -> x = c)
    let endsWith (c: Char) = nextCharMatches c <|> eof
    
    let removeSpaces (p: Parser<'a, 'b>): Parser<'a, 'b> =
        unicodeSpaces
        >>. p
        .>> unicodeSpaces

    // Parse Words
    let bold: Parser<Block,unit> =
        anyStringBetween (nextCharMatches '*') (nextCharMatches '*')
        |>> Word.Bold
        |>> Paragraph

    let strike: Parser<Block,unit> =
        anyStringBetween (nextCharMatches '~') (nextCharMatches '~')
        |>> Word.Strike
        |>> Paragraph

    let italic: Parser<Block,unit> =
        anyStringBetween (pchar '/') (pchar '/')
        |>> Word.Italic
        |>> Paragraph

    let plain: Parser<Block,unit> =
        anyStringBetween (unicodeNewline <|> pchar ' ' <|> lookAhead (anyChar)) (unicodeNewline <|> pchar ' ')
        |>> Word.Plain
        |>> Paragraph
    
    let text =
        choice [
            bold
            strike
            italic
            plain
        ]
    let readLine =
        manyTill text newline
        |>> List.map Block

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
    //let block =
    //    removeSpaces (restOfLine true)
    //    |>> Item.Block
    let block = removeSpaces readLine

    let blocks =
        manyTill block (endsWith '*')
        |>> List.concat
        
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
        .>>. blocks
        .>>. sections

    let execute (content: string) =
        let x = run parser content
        run parser content
        
