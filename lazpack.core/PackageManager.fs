module lazpack.core.PackageManager

let install pkg =
    async {
        let! success = FileManager.installPackage pkg
        if success then
            do! Db.setPkgInstalled pkg.InstalledFileName
            
        return success
    }