module lazpack.cli.Program

open CommandLine
open lazpack.cli
open lazpack.core
open lazpack.core.Utils

// exit codes:
// 0 - success, clean exit
// 1 - match expr fell back to discard - we haven't matched every expr
// 2 - args failed to parse, errors have been logged to console by the parser !! no manual logging required here !!
// 3 - repo fetch failed
// 4 - validation failed
// 5 - package with name not found
// 6 - package fetch failed
// 7 - remanage failed

let handleValidation () =
    let db = DbIo.getDb () |> Async.RunSynchronously
    let validationResult = FileManager.validateInstalledFiles db

    match validationResult with
    | FileManager.AsExpected -> true
    | FileManager.SomeUntracked untrackedFiles ->
        printfn
            $"%s{Escapes.magenta}Found %i{untrackedFiles.Length} untracked files. Be careful mixing lazpack with manual ruleset management!"

        true
    | FileManager.MissingFiles files ->
        printfn
            $"%s{Escapes.red}!! MISSING FILES !! The following files are managed by lazpack, yet are missing - please run `lazpack remanage` to fix:"

        files
        |> List.iter (fun p -> printfn $" - %s{p.Name} (%s{p.InstalledFileName})")

        false

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

    | :? InstallOptions as installParsed ->
        if handleValidation () then
            let pkgRes =
                Db.packageByName installParsed.package
                |> Async.RunSynchronously

            match pkgRes with
            | None ->
                printfn $"[PKG] Could not find package with name `%s{installParsed.package}`"
                5
            | Some pkg ->
                let instRes =
                    PackageManager.install pkg
                    |> Async.RunSynchronously

                if instRes then
                    0
                else
                    printfn
                        "[PKG] Failed to fetch package, please try again later, update repos, or check with the repo maintainer"

                    6
        else
            4

    | :? RemoveOptions as removeParsed ->
        let pkgRes =
            Db.packageByName removeParsed.package
            |> Async.RunSynchronously

        match pkgRes with
        | None ->
            printfn $"[PKG] Could not find package with name `%s{removeParsed.package}`"
            5
        | Some pkg ->
            PackageManager.remove pkg
            |> Async.RunSynchronously

            0

    | :? RemanageOptions as remanageParsed ->
        let res =
            PackageManager.remanage remanageParsed.nuke
            |> Async.RunSynchronously

        if res then
            0
        else
            if remanageParsed.nuke then
                printfn "[FILE] Remanage failed. Please backup & try again with --nuke."
            else
                printfn "[FILE] Remanage failed. Please send your DB in an issue report."

            7

    | _ ->
        printfn "[ARGS] incomplete matches along `match` expr - report this ASAP cause this is very wrong"
        printfn $"[DEBUG] %s{rawParsed.GetType().FullName}"
        1

[<EntryPoint>]
let main args =
    let parseResult =
        Parser.Default.ParseArguments<ListOptions, UpdateOptions, RepoAddOptions, RepoListOptions, InstallOptions, RemoveOptions, RemanageOptions>
            args

    match parseResult with
    | :? Parsed<obj> as objParsed -> argSuccess objParsed.Value
    | :? NotParsed<obj> -> 2
    | _ ->
        printfn "[ARGS] Either F# `match` exprs or CommandLineParser.Fsharp has broken, needs urgent investigation"
        1