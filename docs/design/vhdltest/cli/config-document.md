### ConfigDocument

![Cli Structure](CliView.svg)

#### Purpose

`ConfigDocument` is responsible for deserializing the YAML configuration file that specifies
the VHDL test suite to run. It exposes the list of VHDL source file paths and test bench
patterns required to execute a test run.

#### Data Model

**Files**: `string[]` — Array of VHDL source file paths to compile, as read from the YAML
`files` key. Defaults to an empty array when the key is absent or explicitly set to null.

**Tests**: `string[]` — Array of VHDL test bench entity names to execute, as read from the YAML
`tests` key. Defaults to an empty array when the key is absent or explicitly set to null.

#### Key Methods

**ReadFile**: Reads and deserializes the YAML configuration file at the given path.

- *Parameters*: `string filename` — path to the YAML configuration file.
- *Returns*: `ConfigDocument` — a fully populated document instance.
- *Preconditions*: `filename` must be a readable file path containing valid YAML that can be
  deserialized into a `ConfigDocument`.
- *Postconditions*: Returns a `ConfigDocument` with `Files` and `Tests` populated from the
  YAML content; throws if the file is absent or the content cannot be deserialized.

Reads the file content via `File.ReadAllText`, constructs a YamlDotNet `DeserializerBuilder`
with `HyphenatedNamingConvention` (so YAML keys use hyphens, while C# properties use
PascalCase — for example, a hypothetical multi-word property `SomeKey` would map from the
YAML key `some-key`), then deserializes the content into a `ConfigDocument` instance.
Throws `InvalidOperationException` if the deserialized result is null.
After the null check, `ReadFile` applies null-coalescing guards (`??= []`) to `Files` and
`Tests` so that explicit YAML nulls (`files: null`) are normalized to empty arrays, matching
the behavior of absent keys.

#### Error Handling

`ReadFile` throws `ArgumentNullException` if `filename` is null (via
`ArgumentNullException.ThrowIfNull`). This is a programmer-error guard, not a user-facing
runtime error. `ReadFile` propagates `FileNotFoundException` from `File.ReadAllText` if
the specified file does not exist. It throws `InvalidOperationException` if the
deserialization yields null or if any exception is raised during deserialization (not only
YamlDotNet exceptions — all exceptions from the deserialization call are caught and wrapped).
All three exception types propagate through `Options.Parse` to `Program.Run`, which catches
and reports them via `Context.WriteError`.

#### Dependencies

- **YamlDotNet** — provides `DeserializerBuilder`, `HyphenatedNamingConvention`, and YAML
  deserialization.
- **System.IO.File** — reads the raw YAML content from disk.

#### Callers

- **Options** — calls `ConfigDocument.ReadFile(args.ConfigFile)` inside `Options.Parse`.
