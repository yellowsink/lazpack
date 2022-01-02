module lazpack.core.Types

open MessagePack

// these types are mostly classes instead of records so that MessagePack-CSharp doesnt blow up

type Repo = string

type Package(nIn, duIn, iIn) =
    member this.Name: string = nIn
    member this.DownloadUrl: string = duIn
    member this.Installed: bool = iIn

[<MessagePackObject>]
type Db(rIn, pIn) =
    [<Key(0)>]
    member this.Repos: Repo [] = rIn

    [<Key(1)>]
    member this.Packages: Package [] = pIn