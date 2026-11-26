## Cli

![Cli Structure](CliView.svg)

### Overview

The Cli subsystem implements command-line interface handling for VHDLTest. It encompasses
all logic for parsing command-line arguments, reading configuration files, and managing
output channels. It also answers the built-in `help` command. It contains four units: Context,
ConfigDocument, Options, and HelpCommand.

### Interfaces

**Context.Create**: Static factory method that parses the raw command-line argument array and
returns a fully initialized Context instance.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static Context Create(string[] args)` — returns a `Context` exposing all parsed
  flags (`Version`, `Help`, `Silent`, `Verbose`, `ExitZero`, `Validate`, `Depth`), paths
  (`ConfigFile`, `ResultsFile`, `Simulator`, `LogFile`), I/O methods (`Write`, `WriteVerboseLine`,
  `WriteLine`, `WriteError`), and the computed property `ExitCode`. `Context` implements
  `IDisposable`.
- *Constraints*: Throws `InvalidOperationException` on unrecognized flags or missing required
  argument values. Callers must dispose the returned Context to close any open log-file writer.
- *Notes*: Calling `WriteError` increments the internal error counter, which causes `ExitCode`
  to return `1`. `Program` reads `ExitCode` after the run and assigns it to
  `Environment.ExitCode` before exiting.

**ConfigDocument.ReadFile**: Static factory method that deserializes a YAML configuration file
into a ConfigDocument.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static ConfigDocument ReadFile(string filename)` — returns a `ConfigDocument`
  with `Files` and `Tests` string arrays populated from the YAML file.
- *Constraints*: Throws `FileNotFoundException` if the file does not exist; throws
  `InvalidOperationException` if the content is invalid or cannot be deserialized.

**Options.Parse**: Static factory method that resolves run options from a parsed Context.

- *Type*: In-process .NET public API
- *Role*: Provider
- *Contract*: `static Options Parse(Context args)` — reads `Context.ConfigFile`, calls
  `ConfigDocument.ReadFile`, resolves the working directory from the absolute path of the
  configuration file, and returns an `Options` record exposing `Config` (the deserialized
  `ConfigDocument`) and `WorkingDirectory` (the directory containing the config file). See
  *Options Design* for the full Options record structure.
- *Constraints*: Throws `InvalidOperationException` if no config file is specified or if the
  config file path cannot be resolved to a containing directory. Throws `FileNotFoundException`
  (propagated from `ConfigDocument.ReadFile`) if the specified configuration file does not exist.

**HelpCommand.TryRun**: Answers the `help` command from the help cards embedded in the tool
assembly.

- *Type*: In-process .NET instance method (on the shared `HelpCommand.Default` instance)
- *Role*: Provider
- *Contract*: `bool TryRun(string[] args, out int exitCode)` — when the first argument is `help`,
  writes the answer to the console, returns `true`, and sets the exit code (see *Help Command* in
  the system design); otherwise returns `false` without writing anything or building the catalog.
- *Constraints*: Does not throw for user input.

**HelpCommand.Footer**: The sentence that points a reader of the usage text to the help command.

- *Type*: In-process .NET instance property
- *Role*: Provider
- *Contract*: Returns the one-line pointer that HelpCards supplies, without building the catalog.

### Dependencies

`ConfigDocument.ReadFile` uses **YamlDotNet** for YAML parsing. See *YamlDotNet
Integration Design* for the YamlDotNet integration design. `HelpCommand` uses
**DemaConsulting.HelpCards** to serve the help cards. See *HelpCards Integration Design*.

### Design

The Cli subsystem assembles as follows:

1. `Program.Main` first offers the raw argument array to `HelpCommand.TryRun`. When the first
   argument is `help`, `HelpCommand` answers from the help cards and the invocation ends with its
   exit code; no `Context` is created. `HelpCommand` therefore writes its answers to the console
   itself, and `Context` and `HelpCommand` do not call each other.
2. `Context.Create(args)` receives the raw argument array, parses all flags and positional
   arguments, and returns an initialized `Context` instance. If `--log` was provided, a
   `StreamWriter` is opened for the log file and owned by the `Context`.
3. For a normal test run, `Options.Parse(context)` reads `Context.ConfigFile`, calls
   `ConfigDocument.ReadFile` to deserialize the YAML configuration, and resolves the
   working directory from the absolute path of the configuration file.
4. `Context.ExitCode` returns `1` if any errors were reported via `WriteError`; callers
   apply this to `Environment.ExitCode` before exiting.
