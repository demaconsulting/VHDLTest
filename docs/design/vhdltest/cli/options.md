### Options

#### Purpose

`Options` is a record that holds the fully resolved configuration for a VHDLTest run. It
combines the working directory (derived from the configuration file path) and the deserialised
`ConfigDocument`, providing all information needed by the Simulators subsystem to execute tests.

#### Data Model

**WorkingDirectory**: `string` — The absolute directory path derived from the configuration
file path. All relative VHDL file paths in `ConfigDocument.Files` are resolved relative to
this directory during test execution.

**Config**: `ConfigDocument` — The deserialised configuration document containing the `Files`
and `Tests` arrays.

#### Key Methods

**Parse**: Constructs an `Options` record from a parsed `Context`.

- *Parameters*: `Context args` — a fully initialised Context from which `ConfigFile` is read.
- *Returns*: `Options` — a record with `WorkingDirectory` and `Config` populated.
- *Preconditions*: `args.ConfigFile` must be non-null and must point to a valid, readable YAML
  configuration file.
- *Postconditions*: `WorkingDirectory` holds the absolute directory containing the
  configuration file; `Config` holds the deserialised `ConfigDocument`.

Verifies that `args.ConfigFile` is non-null, calls `ConfigDocument.ReadFile` to deserialise
the YAML content, resolves the full path of the config file via `Path.GetFullPath`, and
extracts the parent directory via `Path.GetDirectoryName`. Throws `InvalidOperationException`
if the directory cannot be resolved.

#### Error Handling

`Parse` throws `InvalidOperationException` if `args.ConfigFile` is null ("Configuration file
not specified"). It propagates `FileNotFoundException` and `InvalidOperationException` from
`ConfigDocument.ReadFile` for missing or invalid configuration files. `Path.GetDirectoryName`
returning null (indicating a root path such as `/` or `C:\` with no parent directory) also throws
`InvalidOperationException`; this is a defensive guard ensuring `WorkingDirectory` is always a
valid, absolute path before it reaches downstream units. This null-return path is unreachable with
well-formed file system paths on supported operating systems and is not directly unit-testable. All
exceptions propagate to `Program.Run`, which catches and reports them via `Context.WriteError`.

#### Dependencies

- **ConfigDocument** — deserialised configuration loaded by `Parse`.
- **System.IO.Path** — resolves the absolute configuration file path and extracts its parent
  directory.

#### Callers

- **Program** — calls `Options.Parse(context)` in `Run` to obtain the options before executing
  the test run.
