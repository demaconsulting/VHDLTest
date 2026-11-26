---
title: Command-line options
summary: Every option, test-bench names, and the order in which a run handles them.
aliases:
  - options
  - flags
  - usage
  - arguments
  - filter
tags:
  - reference
order: 2
see-also:
  - configuration
  - simulators
  - test-results
  - troubleshooting
examples:
  - name: usage
    kind: command
    language: shell
    description: Print the short usage text. It ends with a pointer to this help.
    content: '{{tool}} --help'
    expect: "Run '{{tool}} help' for topics and examples."
  - name: filter
    kind: command
    language: shell
    description: Run only the named test bench.
    content: '{{tool}} --config tests.yaml --simulator mock full_adder_pass_tb'
    expect: 'Passed 1 of 1 tests'
  - name: exit-zero
    kind: command
    language: shell
    description: Exit 0 even though two benches fail.
    content: '{{tool}} --config tests.yaml --simulator mock --exit-0'
    expect: 'Failed 2 of 4 tests'
  - name: verbose
    kind: command
    language: shell
    description: Show every simulator line and step.
    content: '{{tool}} --config tests.yaml --simulator mock --verbose full_adder_pass_tb'
    expect: 'Starting Mock compile...'
  - name: silent-log
    kind: command
    language: shell
    description: Hide the console output and keep it in a log file.
    content: '{{tool}} --silent --log output.log --version'
  - name: unknown-option
    kind: command
    language: shell
    description: An unknown option is an error, printed even with --silent. Exits 1.
    content: '{{tool}} --silent --unknown'
    exit: 1
    expect: 'Error: Unsupported option --unknown'
---
Options start with `-` or `--` and may be given in any order.

| Option | Meaning |
| --- | --- |
| `-v`, `--version` | Print only the version. |
| `-h`, `-?`, `--help` | Print the short usage text. |
| `--silent` | Hide console output. Errors still set exit code 1. |
| `--verbose` | Also print every simulator line and step. |
| `-l`, `--log <file>` | Also write the output to a file. |
| `-c`, `--config <file>` | The configuration file; see [[configuration]]. |
| `-s`, `--simulator <name>` | The simulator; see [[simulators]]. |
| `-r`, `--result`, `--results <file>` | Save the results as `.trx` or `.xml`; see [[test-results]]. |
| `-0`, `--exit-0` | Exit 0 even when test benches fail. |
| `--validate` | Run the self-tests; see [[self-validation]]. |
| `--depth <n>` | First heading level of the self-test report, 1 or more; default 1. |
| `--` | Every later word is a test-bench name. |

**Test benches.** Any word that is not an option is a test-bench name.
The names replace the `tests` list of the configuration file and run in
the order given, as [[#filter]] shows. They are not checked against the
file. Put a name that starts with `-` after `--`.

**Order.** All arguments are read first. `--version`, when all arguments
are valid, prints only the version and skips everything else. Without
it, the tool prints a banner, then handles the first of `--help`,
`--validate`, or a test run. A test run needs `--config`; without it the
tool prints `Error: Missing arguments` and the usage text and exits 1.
The command that [[#silent-log]] prints writes the version to the log
file only.

**Errors.** An unknown option, an option without its value, or a
`--depth` that is not a whole number of 1 or more prints `Error: ...`
and exits 1 ([[#unknown-option]]).
These errors are found before `--silent` and `--log` take effect, so
they are always printed and never logged. Other errors are hidden by
`--silent`, still set exit code 1, and are written to the log file.

**Help.** `help` is a command, not an option, and must be the first
argument. `--silent` and `--log` do not apply to it, and an unknown topic
exits 2. To run a test bench named `help`, put it after the options, as
in `{{tool}} -c tests.yaml help`, or after `--`.
