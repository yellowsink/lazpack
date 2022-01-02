module lazpack.core.Types

open MessagePack

// these types are mostly classes instead of records so that MessagePack-CSharp doesnt blow up

// if 3 unsigned 16bit ints isn't enough to hold your version numbers you are mentally deranged
type Version =
    { major: uint16
      minor: uint16
      rev: uint16 }
    override this.ToString() =
        $"%i{this.major}.%i{this.minor}.%i{this.rev}"

    static member Parse(str: string) =
        match str.Split "." with
        | [| ma; mi |] ->
            { major = uint16 ma
              minor = uint16 mi
              rev = uint16 0 }
        | [| ma; mi; re |] ->
            { major = uint16 ma
              minor = uint16 mi
              rev = uint16 re }
        | _ -> failwithf $"Version string `%s{str}` could not be parsed to a version"

type Package(nIn, vIn, duIn, iIn) =
    member val Name: string = nIn
    member val Version: Version = vIn
    member val DownloadUrl: string = duIn
    member val Installed: bool = iIn

type Repo(nIn, pIn) =
    member val Name: string = nIn
    member val Packages: Package [] = pIn

[<MessagePackObject>]
type Db(rIn) =
    [<Key(0)>]
    member val Repos: Repo [] = rIn

    [<IgnoreMember>]
    member this.RepoPackagePairs =
        this.Repos
        |> Array.map (fun r -> r.Packages |> Array.map (fun p -> (p, r)))
        |> Array.concat
        
    [<IgnoreMember>]
    member this.Packages =
        this.Repos
        |> Array.map (fun r -> r.Packages)
        |> Array.concat