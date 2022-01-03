module lazpack.core.Fetcher

open System.Net
open System.Net.Http
open System.Text.Json
open lazpack.core.Types
open lazpack.core.Utils.Operators

let private fetch (url: string) =
    async {
        let client = new HttpClient()
        let! resp = client.GetAsync url |> Async.AwaitTask

        if resp.StatusCode = HttpStatusCode.OK then
            return!
                resp.Content.ReadAsStringAsync()
                |> Async.AwaitTask
        else
            return ""
    }

// type annotations go brr
let private parse: string -> Repo = JsonSerializer.Deserialize

let private fetchAndParse = fetch |>> parse

let fetchRepo url =
    async {
        let! db = fetchAndParse url
        // ew mutability
        db.Url <- url
        return db
    }

let fetchPackage (pkg: Package) =
    async {
        let client = new HttpClient()
        let! resp = client.GetAsync pkg.DownloadUrl |> Async.AwaitTask
        
        if resp.IsSuccessStatusCode then
            return ValueSome(resp.Content)
        else
            return ValueNone
    }