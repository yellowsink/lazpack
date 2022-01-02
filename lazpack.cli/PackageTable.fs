module lazpack.cli.PackageTable

open System.Text
open lazpack.core.Types
open lazpack.core.Utils.Escapes

/// formats a list of packages as a nice table
let createTable (packages: (Package * Repo) []) =
    let maxPkgNameWidth =
        packages
        |> Array.fold (fun working (p, _) -> max working p.Name.Length) 0

    let maxVerWidth =
        packages
        |> Array.fold (fun working (p, _) -> max working (string p.Version).Length) 0

    let maxRepoNameWidth =
        packages
        |> Array.fold (fun working (_, r) -> max working r.Name.Length) 0

    let headerName = "Name".PadRight maxPkgNameWidth
    let headerVer  = "Version".PadRight maxVerWidth
    let headerRepo = "Repo".PadRight maxRepoNameWidth

    let sb =
        StringBuilder(headerName + headerVer + headerRepo)
        
    sb.AppendLine() |> ignore

    packages
    |> Array.iter
        (fun (p, r) ->
            sb
                .Append(red)
                .Append(p.Name.PadRight maxPkgNameWidth)
                .Append(blue)
                .Append((string p.Version).PadRight maxVerWidth)
                .Append(green)
                .Append(r.Name.PadRight maxRepoNameWidth)
                .AppendLine(defaults)
            |> ignore)

    string sb