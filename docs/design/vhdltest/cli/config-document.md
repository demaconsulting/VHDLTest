### ConfigDocument

#### Purpose

`ConfigDocument` is responsible for deserialising the YAML configuration file that specifies
the VHDL test suite to run. It exposes the list of VHDL source file paths and test bench
patterns required to execute a test run.

#### Data Model

**Files**: `string[]` — Array of VHDL source file paths to compile, as read from the YAML
`files` key. Defaults to an empty array when the key is absent.

**Tests**: `string[]` — Array of test bench file patterns to execute, as read from the YAML
`tests` key. Defaults to an empty array when the key is absent.

#### Key Methods

**ReadFile**: Reads and deserialises the YAML configuration file at the given path.

- *Parameters*: `string filename` — path to the YAML configuration file.
- *Returns*: `ConfigDocument` — a fully populated document instance.
- *Preconditions*: `filename` must be a readable file path containing valid YAML that can be
  deserialised into a `ConfigDocument`.
- *Postconditions*: Returns a `ConfigDocument` with `Files` and `Tests` populated from the
  YAML content; throws if the file is absent or the content cannot be deserialised.

Reads the file content via `File.ReadAllText`, constructs a YamlDotNet `DeserializerBuilder`
with `HyphenatedNamingConvention` (so YAML keys use hyphens), then deserialises the content
into a `ConfigDocument` instance. Throws `InvalidOperationException` if the deserialised result
is null.

#### Error Handling

`ReadFile` propagates `FileNotFoundException` from `File.ReadAllText` if the specified file
does not exist. It throws `InvalidOperationException` if the YAML content is invalid, cannot be
deserialised into a `ConfigDocument`, or the deserialised result is null. Both exception types
propagate through `Options.Parse` to `Program.Run`, which catches and reports them via
`Context.WriteError`.

#### Dependencies

- **YamlDotNet** — provides `DeserializerBuilder`, `HyphenatedNamingConvention`, and YAML
  deserialisation.
- **System.IO.File** — reads the raw YAML content from disk.

#### Callers

- **Options** — calls `ConfigDocument.ReadFile(args.ConfigFile)` inside `Options.Parse`.
