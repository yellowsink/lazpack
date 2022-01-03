module lazpack.core.Types

open MessagePack
open lazpack.core.Utils

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

[<MessagePackObject>]
type Package(nIn, vIn, duIn, iIn) =
    new() = Package("", Version.Parse "0.0.0", "", false)

    [<Key(0)>]
    member val Name: string = nIn with get, set

    [<IgnoreMember>]
    member val Version: Version = vIn with get, set

    [<Key(1)>]
    member this.VersionSer
        with get () = string this.Version
        and set str = this.Version <- Version.Parse str

    [<Key(2)>]
    member val DownloadUrl: string = duIn with get, set

    [<Key(3)>]
    member val Installed: bool = iIn with get, set
    
    [<IgnoreMember>]
    member this.InstalledFileName = $"lazpackRuleset-%s{Misc.sanitise this.Name}.dll"

[<MessagePackObject>]
type Repo(nIn, uIn, pIn) =
    new() = Repo("", "", [||])

    [<Key(0)>]
    member val Name: string = nIn with get, set
    
    [<Key(1)>]
    member val Url: string = uIn with get, set

    [<Key(2)>]
    member val Packages: Package [] = pIn with get, set

[<MessagePackObject>]
type Db(rIn) =
    new() = Db([||])

    [<Key(0)>]
    member val Repos: Repo [] = rIn with get, set

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