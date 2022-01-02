module lazpack.cli.Program

open CommandLine
open lazpack.cli
open lazpack.core
open lazpack.core.Types

// exit codes:
// 0 - success, clean exit
// 1 - match expr fell back to discard - we haven't matched every expr
// 2 - args failed to parse, errors have been logged to console by the parser !! no manual logging required here !!

let argSuccess (rawParsed: obj) =
    match rawParsed with
    | :? ListOptions ->
        printfn "Fetching db..."
        //let db = DbIo.getDb() |> Async.RunSynchronously
        let db = Db([|
            Repo("Sink's cool repo 1", [|
                Package("Tau", Version.Parse "2021.1127.0", "https://te.st", false)
            |])
            Repo("Another repo", [|
                Package("Sentakki", Version.Parse "6.9.420", "https://te.st", true)
            |]);
        |])
        let table = PackageTable.createTable db.RepoPackagePairs
        printfn $"%s{table}"
        0
    | :? RepoAddOptions as repoAddParsed ->
        printfn $"Chose to add repo with url %s{repoAddParsed.url}"
        0
    | _ ->
        printfn "[ARGS] incomplete matches along `match` expr - report this ASAP cause this is very wrong"
        printfn $"[DEBUG] %s{rawParsed.GetType().FullName}"
        1

[<EntryPoint>]
let main args =
    let parseResult =
        Parser.Default.ParseArguments<ListOptions, RepoAddOptions> args

    match parseResult with
    | :? Parsed<obj> as objParsed -> argSuccess objParsed.Value
    | :? NotParsed<obj> -> 2
    | _ ->
        printfn "[ARGS] Either F# `match` exprs or CommandLineParser.Fsharp has broken, needs urgent investigation"
        1