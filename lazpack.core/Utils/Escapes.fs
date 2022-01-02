module lazpack.core.Utils.Escapes

let private ESC = "\u001b["

let defaults = ESC + "39;49m"
let black    = ESC + "30m"
let red      = ESC + "31m"
let green    = ESC + "32m"
let yellow   = ESC + "33m"
let blue     = ESC + "34m"
let magenta  = ESC + "35m"
let cyan     = ESC + "36m"
let white    = ESC + "37m"