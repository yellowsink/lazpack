namespace lazpack.core

open System.Net
open System.Net.Http

type ruleset = string

type repo = { url: string; rulesets: ruleset list }

module Fetcher =
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