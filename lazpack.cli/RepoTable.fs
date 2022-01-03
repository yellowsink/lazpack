module lazpack.cli.RepoTable

open System.Text
open lazpack.core.Types
open lazpack.core.Utils.Escapes

let installedPkgCount (repo: Repo) =
    repo.Packages
    |> Array.filter (fun p -> p.Installed)

/// formats a list of repos as a nice table
let createTable (repos: Repo []) =
    let maxNameWidth =
        1
        + (max
            4
            (repos
             |> Array.map (fun r -> r.Name.Length)
             |> Array.max))

    let maxPkgsWidth =
        1
        + (max
            4
            (repos
             |> Array.map (fun r -> (string r.Packages.Length).Length)
             |> Array.max))

    let maxInstWidth =
        1
        + (max
            9
            (repos
             |> Array.map (fun r -> (string (installedPkgCount r).Length).Length)
             |> Array.max))

    let headerName = "Name".PadRight maxNameWidth
    let headerPkgs = "Pkgs".PadRight maxPkgsWidth
    let headerInst = "Installed".PadRight maxInstWidth

    let sb =
        StringBuilder(headerName + headerPkgs + headerInst)

    sb.AppendLine() |> ignore

    repos
    |> Array.iter
        (fun r ->
            sb
                .Append(red)
                .Append(r.Name.PadRight maxNameWidth)
                .Append(blue)
                .Append((string r.Packages.Length).PadRight maxPkgsWidth)
                .Append(green)
                .Append(
                    (string (installedPkgCount r).Length)
                        .PadRight maxInstWidth
                )
                .AppendLine(defaults)
            |> ignore)

    string sb