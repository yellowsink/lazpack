namespace lazpack.cli

open CommandLine

[<Verb("list", HelpText = "Lists all packages available")>]
// this is kinda hacky but its the easiest way to make an empty type in F#
// commandline refuses to accept `unit` as a type for a verb
// and `obj` will instantly match literally anything, so can't be used in a `match` expr
// so here we are: an empty class, with a constructor, as records cannot be empty
type ListOptions() =
    class
    end
    
[<Verb("update", HelpText = "Updates local package cache from repos")>]
type UpdateOptions() =
    class
    end

[<Verb("repoadd", HelpText = "Adds a repo")>]
type RepoAddOptions =
    { [<Option('u', "url", Required = true)>]
      url: string }