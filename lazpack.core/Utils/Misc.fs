module lazpack.core.Utils.Misc

/// https://www.mtu.edu/umc/services/websites/writing/characters-avoid/
let badCharacters =
    [ '#'
      '%'
      '&'
      '{'
      '}'
      '\\'
      '<'
      '>'
      '*'
      '?'
      '/'
      ' '
      '$'
      '!'
      '\''
      '"'
      ':'
      '@'
      '+'
      '`'
      '|'
      '=' ]

let replacement = '_'

let sanitise (raw: string) =
    raw.ReplaceLineEndings ""
    |> String.map
        (fun c ->
            if badCharacters |> List.contains c then
                c
            else
                replacement)