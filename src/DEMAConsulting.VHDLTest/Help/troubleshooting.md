---
title: Errors and exit codes
summary: What the exit codes mean, and how to fix the common error messages.
aliases:
  - errors
  - exit-codes
  - problems
tags:
  - troubleshooting
order: 7
see-also:
  - command-line
  - configuration
  - simulators
  - test-results
examples:
  - name: no-config
    kind: command
    language: shell
    description: A test run without --config. Exits 1.
    content: '{{tool}} --simulator mock'
    exit: 1
    expect: 'Error: Missing arguments'
  - name: missing-file
    kind: command
    language: shell
    description: A configuration file that does not exist. Exits 1.
    content: '{{tool}} --config missing.yaml --simulator mock'
    exit: 1
    expect: 'Could not find file'
  - name: bad-config
    kind: command
    language: shell
    description: A configuration with the key test instead of tests. Exits 1.
    content: '{{tool}} --config misspelled.yaml --simulator mock'
    exit: 1
    expect: 'Error: Configuration document misspelled.yaml is invalid'
  - name: unknown-simulator
    kind: command
    language: shell
    description: A simulator name that does not exist. Exits 1.
    content: '{{tool}} --config tests.yaml --simulator unknown'
    exit: 1
    expect: 'Error: Simulator not found'
  - name: bad-depth
    kind: command
    language: shell
    description: A --depth below 1. Exits 1.
    content: '{{tool}} --validate --depth 0'
    exit: 1
    expect: "Error: Invalid value '0' for option '--depth'"
  - name: unknown-topic
    kind: command
    language: shell
    description: A help topic that does not exist. Exits 2.
    content: '{{tool}} help no-such-topic'
    exit: 2
    expect: "Unknown help card 'no-such-topic'"
---
**Exit codes.**

| Code | Meaning |
| --- | --- |
| 0 | Success, or failing benches with `--exit-0`. |
| 1 | An error, a failing bench, or a failed self-test. |
| 2 | A wrong `help` argument or an unknown help topic. |

Errors are printed in red as `Error: ...`. Errors in the arguments are
found before the run starts: they are always printed, even with
`--silent`, and never reach the `--log` file. Other errors are hidden by
`--silent`, still exit 1, and are written to the log file.

**Common messages.**

- `Missing arguments`: a test run needs `--config <file>`
  ([[#no-config]]).
- `Unsupported option`: check the spelling against [[command-line]].
- `Missing ... name` or `Missing depth argument`: the option needs a
  value after it.
- `Invalid value ... for option '--depth'`: use a whole number of 1 or
  more ([[#bad-depth]]).
- `Could not find file`: the configuration file does not exist. The
  message is followed by technical details ([[#missing-file]]).
- `Configuration document <file> is invalid`: the file is not valid
  YAML or has a key other than `files` and `tests`, such as `test:`
  ([[#bad-config]]); see [[configuration]].
- `Simulator not found`: the name is unknown, or no simulator is
  installed ([[#unknown-simulator]]); see [[simulators]].
- `<Name> Simulator not available`: the named simulator is not on
  `PATH` and its folder variable is not set.
- `Build Failed`: a file did not compile, so no bench ran. Run again
  with `--verbose` to see every compiler line and command.
- `Failed to write results file`: the folder of `--results` does not
  exist or the file is locked.
- `Failed <bench>`: the bench reported an error or failure, or the
  simulator exited with an error. The red lines above it show why.
- `Unknown help card`: the topic does not exist. The message lists the
  topics ([[#unknown-topic]]).

The folder of the `--log` file must exist.
