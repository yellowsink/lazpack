module lazpack.cli.Program

open CommandLine
open lazpack.cli

// exit codes:
// 0 - success, clean exit
// 1 - match expr fell back to discard - we haven't matched every expr
// 2 - args failed to parse, errors have been logged to console by the parser !! no manual logging required here !!

let argSuccess (rawParsed: obj) =
    match rawParsed with
    | :? ListOptions ->
        printfn "Chose to list packages"
        0
    | :? RepoAddOptions as repoAddParsed ->
        printfn $"Chose to add repo with url %s{repoAddParsed.url}"
        0
    | _ ->
        printfn "[ARGS] incomplete matches along `match` expr - report this ASAP cause this is very wrong"
        1

[<EntryPoint>]
let main args =
    let parseResult =
        Parser.Default.ParseArguments<ListOptions, RepoAddOptions> args

    match parseResult with
    | :? Parsed<obj> as objParsed -> argSuccess objParsed
    | :? NotParsed<obj> -> 2
    | _ ->
        printfn "[ARGS] Either F# `match` exprs or CommandLineParser.Fsharp has broken, needs urgent investigation"
        1