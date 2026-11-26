---
title: Test results
summary: The console summary of a run, and the TRX or JUnit file that --results writes.
aliases:
  - results
  - trx
  - junit
  - results-file
  - reports
tags:
  - reference
order: 5
see-also:
  - command-line
  - self-validation
examples:
  - name: trx
    kind: command
    language: shell
    description: Run one bench and save the results as a TRX file.
    content: '{{tool}} --config tests.yaml --simulator mock --results results.trx full_adder_pass_tb'
    expect: 'Passed 1 of 1 tests'
  - name: junit
    kind: command
    language: shell
    description: Save the results as JUnit XML instead.
    content: '{{tool}} --config tests.yaml --simulator mock --results results.xml full_adder_pass_tb'
    expect: 'Passed 1 of 1 tests'
  - name: keep-going
    kind: command
    language: shell
    description: Exit 0 and leave the failures to the results file.
    content: '{{tool}} --config tests.yaml --simulator mock --results results.trx --exit-0'
    expect: 'Failed 2 of 4 tests'
  - name: missing-folder
    kind: command
    language: shell
    description: The folder of the results file must exist. Exits 1.
    content: '{{tool}} --config tests.yaml --simulator mock --results missing/results.trx full_adder_pass_tb'
    exit: 1
    expect: 'Error: Failed to write results file'
---
**Console.** For each bench the tool prints `Starting <bench>`, the
error, warning, and note lines of the simulator, and then
`Passed <bench> (<n> seconds)` or `Failed <bench> (<n> seconds)`.
A summary at the end repeats these lines and counts them, as in
`Passed 2 of 4 tests` and `Failed 2 of 4 tests`. The exit code is 1
when a bench failed, unless `--exit-0` is given.

**Results file.** `--results <file>` also saves the outcome of every
bench, as [[#trx]] shows. The extension chooses the format: `.xml`
writes JUnit XML ([[#junit]]), and any other extension writes a Visual
Studio TRX file. Both are read by the test-report tasks of CI systems.

Each bench is one test result, named after the bench (also its class
name), with its outcome, start time, duration, and the full simulator
output. A failed bench also carries its error lines as the error
message.

**When it is written.** The file is written after all benches ran,
also when some failed. It is not written when the build fails, because
no bench ran. The folder of the file must exist; otherwise the tool
prints `Error: Failed to write results file: ...` and exits 1
([[#missing-folder]]). An existing file is replaced.

**In CI.** With `--exit-0` the step stays green while the results file
records the failures, so a test-report task can show them
([[#keep-going]]). Without it, a failing bench fails the step.
