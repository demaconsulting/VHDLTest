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
  (`ConfigFile`, `ResultsFile`, `Simulator`, `LogFile`), I/O methods (`Write`, `WriteVerboseLine`,
  `WriteLine`, `WriteError`), and the computed property `ExitCode`. `Context` implements
  `IDisposable`.
- *Constraints*: Throws `InvalidOperationException` on unrecognised flags or missing required
  argument values. Callers must dispose the returned Context to close any open log-file writer.
- *Notes*: Calling `WriteError` increments the internal error counter, which causes `ExitCode`
  to return `1`. `Program` reads `ExitCode` after the run and assigns it to
  `Environment.ExitCode` before exiting.

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
  configuration file, and returns an `Options` record exposing `Config` (the deserialised
  `ConfigDocument`) and `WorkingDirectory` (the directory containing the config file). See
  *Options Design* for the full Options record structure.
- *Constraints*: Throws `InvalidOperationException` if no config file is specified or if the
  config file path cannot be resolved to a containing directory. Throws `FileNotFoundException`
  (propagated from `ConfigDocument.ReadFile`) if the specified configuration file does not exist.

### Dependencies

`ConfigDocument.ReadFile` uses **YamlDotNet** for YAML parsing. See *YamlDotNet
Integration Design* for the YamlDotNet integration design.

### Design

The Cli subsystem assembles as follows:

1. `Context.Create(args)` receives the raw argument array, parses all flags and positional
   arguments, and returns an initialised `Context` instance. If `--log` was provided, a
   `StreamWriter` is opened for the log file and owned by the `Context`.
2. For a normal test run, `Options.Parse(context)` reads `Context.ConfigFile`, calls
   `ConfigDocument.ReadFile` to deserialise the YAML configuration, and resolves the
   working directory from the absolute path of the configuration file.
3. `Context.ExitCode` returns `1` if any errors were reported via `WriteError`; callers
   apply this to `Environment.ExitCode` before exiting.
