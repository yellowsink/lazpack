module lazpack.core.Utils.Operators

/// async to sync compose operator
let (|>>) a b =
    fun c ->
        async {
            let! aRes = a c
            return b aRes
        }

/// async to async compose operator
let (>|>) a b =
    fun c ->
        async {
            let! aRes = a c
            return! b aRes
        }

/// sync to async compose operator
let (>>|) a b = fun c -> async { return! b (a c) }