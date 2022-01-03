module lazpack.cli.Program

open CommandLine
open lazpack.cli
open lazpack.core

// exit codes:
// 0 - success, clean exit
// 1 - match expr fell back to discard - we haven't matched every expr
// 2 - args failed to parse, errors have been logged to console by the parser !! no manual logging required here !!
// 3 - repo fetch failed

let argSuccess (rawParsed: obj) =
    match rawParsed with
    | :? ListOptions ->
        let db = DbIo.getDb () |> Async.RunSynchronously

        let table =
            PackageTable.createTable db.RepoPackagePairs

        printfn $"%s{table}"
        0

    | :? RepoAddOptions as repoAddParsed ->
        match Fetcher.fetchRepo repoAddParsed.url
              |> Async.Catch
              |> Async.RunSynchronously with
        | Choice1Of2 repo -> Db.addRepo repo |> Async.RunSynchronously
        | Choice2Of2 err -> printfn $"[REPO] Failed to fetch repo with error: %s{err.Message}"

        3

    | :? UpdateOptions ->
        Db.updateRepos () |> Async.RunSynchronously
        0

    | :? RepoListOptions ->
        let db = DbIo.getDb () |> Async.RunSynchronously
        let table = RepoTable.createTable db.Repos
        printfn $"%s{table}"
        0
    | _ ->
        printfn "[ARGS] incomplete matches along `match` expr - report this ASAP cause this is very wrong"
        printfn $"[DEBUG] %s{rawParsed.GetType().FullName}"
        1

[<EntryPoint>]
let main args =
    let parseResult =
        Parser.Default.ParseArguments<ListOptions, UpdateOptions, RepoAddOptions, RepoListOptions> args

    match parseResult with
    | :? Parsed<obj> as objParsed -> argSuccess objParsed.Value
    | :? NotParsed<obj> -> 2
    | _ ->
        printfn "[ARGS] Either F# `match` exprs or CommandLineParser.Fsharp has broken, needs urgent investigation"
        1