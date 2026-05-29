## Cli

### Overview

The Cli subsystem implements command-line interface handling for VHDLTest. It encompasses
all logic for parsing command-line arguments, reading configuration files, and managing
output channels. It contains three units: Context, ConfigDocument, and Options.

### Interfaces

**Context.Create**: Static factory method that parses the raw command-line argument array and
returns a fully initialised Context instance.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static Context Create(string[] args)` — returns a `Context` exposing all parsed
  flags (`Version`, `Help`, `Silent`, `Verbose`, `ExitZero`, `Validate`, `Depth`), paths
  (`ConfigFile`, `ResultsFile`, `Simulator`), and I/O methods (`Write`, `WriteVerboseLine`,
  `WriteLine`, `WriteError`). `Context` implements `IDisposable`.
- *Constraints*: Throws `InvalidOperationException` on unrecognised flags or missing required
  argument values. Callers must dispose the returned Context to close any open log-file writer.

**ConfigDocument.ReadFile**: Static factory method that deserialises a YAML configuration file
into a ConfigDocument.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static ConfigDocument ReadFile(string filename)` — returns a `ConfigDocument`
  with `Files` and `Tests` string arrays populated from the YAML file.
- *Constraints*: Throws `FileNotFoundException` if the file does not exist; throws
  `InvalidOperationException` if the content is invalid or cannot be deserialised.

**Options.Parse**: Static factory method that resolves run options from a parsed Context.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static Options Parse(Context args)` — reads `Context.ConfigFile`, calls
  `ConfigDocument.ReadFile`, resolves the working directory from the absolute path of the
  configuration file, and returns an `Options` record.
- *Constraints*: Throws `InvalidOperationException` if no config file is specified or if the
  config file path cannot be resolved to a containing directory.

### Design

The Cli subsystem assembles as follows:

1. `Program` calls `Context.Create(args)` with the raw argument array. Context parses all flags
   and positional arguments and returns an initialised `Context` instance; if `--log` was
   provided, an output `StreamWriter` is opened for the log file.
2. `Program` inspects `Context.Version`, `Context.Help`, and `Context.Validate` to dispatch to
   the appropriate handler (version display, help display, or self-validation).
3. For a normal test run, `Program` calls `Options.Parse(context)`, which reads
   `Context.ConfigFile`, calls `ConfigDocument.ReadFile` to deserialise the YAML configuration,
   and resolves the working directory from the absolute path of the configuration file.
4. `Program` passes the resulting `Options` to the Simulators subsystem to execute the test run.
5. `Context.ExitCode` is set to `1` if any errors were reported via `WriteError`; `Program`
   applies this to `Environment.ExitCode` before exiting.
