module lazpack.cli.Program

open CommandLine
open lazpack.cli

let failMatches () =
    printfn "incomplete matches along F# `match` expression - report this ASAP cause this is very very wrong"
    1

// exit codes:
// 0 - success, clean exit
// 1 - match expr fell back to discard - we haven't matched every expr
// 2 - args failed to parse, errors have been logged to console by the parser !! no manual logging required here !!
[<EntryPoint>]
let main args =
    let parseResult =
        Parser.Default.ParseArguments<ListOptions, RepoAddOptions> args
    
    match parseResult with
    | :? Parsed<obj> as objParsed ->
        match objParsed.Value with
        | :? ListOptions as listParsed ->
            printfn "Chose to list packages"
            0
        | :? RepoAddOptions as repoAddParsed ->
            printfn $"Chose to add repo with url %s{repoAddParsed.url}"
            0
        | _ -> failMatches ()
    | :? NotParsed<obj> -> 2
    | _ -> failMatches ()