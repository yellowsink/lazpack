module lazpack.core.Db

open lazpack.core.Types

let addRepo repo =
    async {
        let! db = DbIo.getDb ()
        // ew mutability
        db.Repos <- db.Repos |> Array.append [|repo|]
        do! DbIo.saveDb db
    }

let updateRepos () = async {
    let! db = DbIo.getDb()
    for i = 0 to db.Repos.Length - 1 do
        // for loops? mutability? wow this is very not functional
        // async "awaits" in a for loop work in F# tho? damn did not expect that...
        let! fetched = Fetcher.fetchRepo db.Repos[i].Url
        db.Repos[i] <- fetched
    
    do! DbIo.saveDb db
}

let setPkgInstalled fName inst =
    async {
        let! db = DbIo.getDb ()
        db.Repos <-
            db.Repos
            |> Array.map (fun r ->
                // yuck, but its the easiest way to do this with classes
                
                Repo(r.Name, r.Url, (r.Packages
                                     |> Array.map (fun p ->
                                         if p.InstalledFileName = fName then
                                             Package(p.Name, p.Version, p.DownloadUrl, inst)
                                         else
                                             p))))
    
        do! DbIo.saveDb db
    }
    
let packageByName name =
    async {
        let! db = DbIo.getDb ()
        
        return db.Packages
              |> Array.sortBy (fun p -> p.Version)
              |> Array.tryFind (fun p -> p.Name = name)
    }