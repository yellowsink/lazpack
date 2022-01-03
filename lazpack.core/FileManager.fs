module lazpack.core.FileManager

open System
open System.IO
open lazpack.core.Types

// based on https://github.com/sink-archive/Lazer2Stable/blob/master/Lazer2Stable/LazerFolderUtils.cs
let lazerRulesetsFolder =
    Path.Combine(
        (match Environment.OSVersion.Platform with
         | PlatformID.Win32NT -> Environment.GetFolderPath Environment.SpecialFolder.LocalApplicationData
         | PlatformID.Unix ->
             Path.Combine((Environment.GetFolderPath Environment.SpecialFolder.UserProfile), ".local/share")
         | p -> failwithf $"[FILE] Ran on unknown platform type `%s{p.ToString()}`, please report this"),
        "osu/rulesets"
    )

type ValidationResult =
    | AsExpected
    | SomeUntracked of string list
    | MissingFiles of Package list

let passed =
    function
    | AsExpected -> true
    | SomeUntracked _ -> true
    | MissingFiles _ -> false

let validateInstalledFiles (db: Db) =
    let files = DirectoryInfo(lazerRulesetsFolder).GetFiles()
                |> Array.map (fun f -> f.Name)

    let missing =
        db.Packages
        |> Array.toList
        |> List.filter
            (fun p ->
                p.Installed
                && not (files |> Array.contains p.InstalledFileName))

    if missing.Length > 0 then
        MissingFiles(missing)
    else
        let untrackedFiles =
            files
            |> Array.toList
            |> List.filter
                (fun f ->
                    not (
                        db.Packages
                        |> Array.exists (fun p -> p.InstalledFileName = f)
                    ))

        if untrackedFiles.Length > 0 then
            SomeUntracked(untrackedFiles)
        else
            AsExpected

let installPackage pkg =
    async {
        let! fetched = Fetcher.fetchPackage pkg

        match fetched with
        | ValueSome content ->
            let fullFileName =
                Path.Combine(lazerRulesetsFolder, pkg.InstalledFileName)

            let stream = File.Create fullFileName
            do! content.CopyToAsync stream |> Async.AwaitTask
            return true
        | ValueNone -> return false
    }