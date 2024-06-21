namespace Page.Org

type Status = TODO | DONE
type ListType = Bulleted | Numbered | Plussed

type Words =
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
and Column = Words option
and Table = Row list

type Language = Language of string

type Block =
    | Quote of string
    | Example of string
    | Code of Language: Language option * Content: string
    | ListItems of string list
    | Table of Table
    | Paragraph of Words

type Tag = Tag of string
