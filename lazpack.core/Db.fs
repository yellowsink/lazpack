module lazpack.core.Db

let addRepo repo =
    async {
        let! db = DbIo.getDb ()
        // ew mutability and OOP *at the same time*
        db.AddRepo repo
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