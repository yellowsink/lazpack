module lazpack.cli.PackageTable

open System.Text
open lazpack.core.Types
open lazpack.core.Utils.Escapes

/// formats a list of packages as a nice table
let createTable (packages: (Package * Repo) []) =
    let maxPkgNameWidth =
        1
        + (packages
           |> Array.map (fun (p, _) -> p.Name.Length)
           |> Array.max)

    let maxVerWidth =
        1
        + (packages
           |> Array.map (fun (p, _) -> (string p.Version).Length)
           |> Array.max)

    let maxRepoNameWidth =
        1
        + (packages
           |> Array.map (fun (_, r) -> r.Name.Length)
           |> Array.max)

    let headerName = "Name".PadRight maxPkgNameWidth
    let headerVer = "Version".PadRight maxVerWidth
    let headerRepo = "Repo".PadRight maxRepoNameWidth

    let sb =
        StringBuilder("    " + headerName + headerVer + headerRepo)

    sb.AppendLine() |> ignore

    packages
    |> Array.iter
        (fun (p, r) ->
            sb
                .Append(cyan)
                .Append(if p.Installed then "(*) " else "( ) ")
                .Append(red)
                .Append(p.Name.PadRight maxPkgNameWidth)
                .Append(blue)
                .Append((string p.Version).PadRight maxVerWidth)
                .Append(green)
                .Append(r.Name.PadRight maxRepoNameWidth)
                .AppendLine(defaults)
            |> ignore)

    string sb