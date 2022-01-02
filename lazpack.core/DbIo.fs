module lazpack.core.DbIo

open System
open System.IO
open MessagePack
open lazpack.core.Types

/// The path to the database file
let dbPath =
    match Environment.OSVersion.Platform with
    | PlatformID.Win32NT ->
        Path.Combine(
            (Environment.GetFolderPath Environment.SpecialFolder.LocalApplicationData),
            "Cain Atkinson/lazpack.db"
        )
    | PlatformID.Unix ->
        Path.Combine((Environment.GetFolderPath Environment.SpecialFolder.UserProfile), ".local/share/lazpack.db")
    | p -> failwithf $"[DBIO] Ran on unknown platform type `%s{p.ToString()}`, please report this"

/// Checks if the DB exists on disk
let dbExistsOnDisk () = File.Exists dbPath

/// Gets the DB from disk, or returns a fresh one if non existent
let getDb () =
    async {
        // a more idiomatic functional approach is to `return!` the whole `if` and wrap the `Db()` in `async.Return`
        // but in contrast to this more imperative approach with separate `return`s, its less clean
        if (File.Exists dbPath) then
            let stream = File.OpenRead dbPath

            let valueTask =
                MessagePackSerializer.DeserializeAsync<Db> stream

            return! valueTask.AsTask() |> Async.AwaitTask
        else
            return Db([||])
    }

/// saves a DB to disk
let saveDb db =
    async {
        let stream = File.OpenWrite dbPath

        return!
            MessagePackSerializer.SerializeAsync(stream, db)
            |> Async.AwaitTask
    }