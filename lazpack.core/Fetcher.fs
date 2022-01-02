module lazpack.core.Fetcher

open System.Net
open System.Net.Http
open System.Text.Json
open lazpack.core.Utils.Operators

type ruleset = string

type repo = { url: string; ruleSets: ruleset list }

let fetch (url: string) =
    async {
        let client = new HttpClient()
        let! resp = client.GetAsync url |> Async.AwaitTask

        return
            (if resp.StatusCode = HttpStatusCode.OK then
                 resp.Content.ToString()
             else
                 "")
    }

// type annotations go brr
let parse: string -> repo = JsonSerializer.Deserialize

let fetchAndParse = fetch |>> parse