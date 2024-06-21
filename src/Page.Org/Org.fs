namespace Page.Org

type Keyword = string * string
type Status = TODO | DONE
type ListType = Bulleted | Numbered | Plussed

type Word =
    | Bold of string
    | Italic of string
    | Highlight of string
    | Underline of string
    | Verbatim of string
    | Strike of string
    | Link of Url: string * Alias: string option
    | Image of Address: string
    | Punctuation of string
    | Plain of string

type Row =
    | Break
    | Column of Column list
and Column = Word option
and Table = Row list

type Language = Language of string

type Block =
    | Quote of string
    | Example of string
    | Code of Language: Language option * Content: string
    | ListItems of string list
    | Table of Table
    | Paragraph of Word

type Tag = Tag of string
type Title = Title of Word list

type Org =
    { Metadata: Metadata
      Document: Document }
and Metadata = Map<string, string>
and Document =
    { Block: Block list
      Section: Section list }
and Section =
    { Status: Status option
      Header: Word option
      Tags: Tag list
      Properties: Map<string, string>
      Title: Title
      Document: Document }

