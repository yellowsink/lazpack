module lazpack.core.PackageManager

open System.IO

let install pkg =
    async {
        let! success = FileManager.installPackage pkg

        if success then
            do! Db.setPkgInstalled pkg.InstalledFileName true

        return success
    }

let remove pkg =
    async {
        FileManager.removePackage pkg
        do! Db.setPkgInstalled pkg.InstalledFileName false
    }

let remanage nuke =
    async {
        let removeFiles () = 
            // remove files
            if nuke then
                Directory.Delete(FileManager.lazerRulesetsFolder, true)
            else
                DirectoryInfo(FileManager.lazerRulesetsFolder)
                    .GetFiles()
                |> Array.iter
                    (fun f ->
                        if f.Name.StartsWith "osu.Game.Rulesets.LAZPACK-" then
                            f.Delete())
        
        removeFiles()

        // re-add files
        let! db = DbIo.getDb ()

        let toAdd =
            db.Packages |> Array.filter (fun p -> p.Installed)

        let failed =
            toAdd
            |> Array.exists (fun p -> not (install p |> Async.RunSynchronously))
            
        if failed then
            removeFiles()
            return false
        else
            return true
    }