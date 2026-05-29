### RunLineRule

#### Purpose

`RunLineRule` is an immutable record pairing a compiled regular expression with the
`RunLineType` to assign when the pattern matches a simulator output line. It is the
rule element that `RunProcessor` applies to classify each captured line.

#### Data Model

| Property  | Type          | Description                                                              |
| --------- | ------------- | ------------------------------------------------------------------------ |
| `Type`    | `RunLineType` | The classification assigned to a line when `Pattern` matches.            |
| `Pattern` | `Regex`       | Compiled regular expression with a 100 ms evaluation timeout.            |

#### Key Methods

**`Create(RunLineType type, string pattern) → RunLineRule`**

Static factory method that compiles `pattern` into a `Regex` with `RegexOptions.None`
and a 100 ms evaluation timeout, then constructs and returns a new `RunLineRule`. The
timeout guards against catastrophic backtracking on pathological input lines.

- *Preconditions*: `pattern` is a syntactically valid regular expression string.
- *Postconditions*: Returns a `RunLineRule` whose `Pattern` is compiled and ready for
  use.

#### Error Handling

`Regex` construction throws `ArgumentException` if `pattern` is syntactically invalid;
this propagates to the caller. During matching in `RunProcessor.Parse`, a
`RegexMatchTimeoutException` is thrown if pattern evaluation exceeds 100 ms; this
propagates to the caller and is not caught within the Run subsystem.

#### Dependencies

- **RunLineType** — the type of the `Type` property.
- **`System.Text.RegularExpressions.Regex`** (OTS — .NET Base Class Library) — used
  for pattern compilation and line matching.

#### Callers

- Simulator implementations in the Simulators subsystem construct `RunLineRule`
  instances via `Create` to build their output classification rule sets.
- `RunProcessor` applies each rule during `Parse` to classify output lines.
