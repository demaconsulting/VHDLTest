---
title: Self-validation
summary: Prove that the tool and a simulator work, and save the evidence as results.
aliases:
  - validate
  - validation
  - qualification
tags:
  - reference
order: 6
see-also:
  - simulators
  - test-results
examples:
  - name: run
    kind: command
    language: shell
    description: Run the self-tests with the mock simulator.
    content: '{{tool}} --validate --simulator mock'
    expect: 'Validation Passed'
  - name: trx
    kind: command
    language: shell
    description: Also write the results as a TRX file.
    content: '{{tool}} --validate --simulator mock --results validation.trx'
    expect: 'Validation Passed'
  - name: depth
    kind: command
    language: shell
    description: Start the report at heading level 2 to embed it in a larger document.
    content: '{{tool}} --validate --simulator mock --depth 2'
    expect: '## DEMAConsulting.VHDLTest'
  - name: real
    kind: snippet
    language: shell
    save-as: validate-ghdl.sh
    description: Validate the tool together with an installed GHDL.
    content: |
      {{tool}} --validate --simulator ghdl --results validation.trx
---
`--validate` runs two tests of the tool itself, so a regulated project
can show that the installed version works on its build machine. Each
test runs the same four embedded full-adder and half-adder test
benches, two that pass and two that fail, in a `validation.tmp` folder
in the current folder that is removed afterwards.
`VHDLTest_TestPasses` passes when the passing benches are reported as
passed, and `VHDLTest_TestFails` passes when the failing benches are
reported as failed. A compile error fails both tests.

**Simulator.** With `--simulator mock` ([[#run]]) the tests need no
installation and prove the tool alone. Name a real simulator, as in
[[#real]], to prove the tool together with that simulator. Without
`--simulator`, the first installed simulator is used; see
[[simulators]].

**Report.** The output is Markdown. It starts with a heading and a table
of the tool version, machine name, operating system, .NET runtime, and
time stamp. Each test prints one line marked `Passed` or `Failed`, then
come `Total Tests`, `Passed`, and `Failed` lines, and
`Validation Passed` when both tests passed. The exit code is 0 only
when both pass. `--depth` sets the level of the first heading, as
[[#depth]] shows.

**Results files.** `--results <file>` also writes the outcome, as in
[[#trx]]: `.xml` writes JUnit XML and any other extension a TRX file.
The folder of the file must exist. `--config` and test-bench names are
ignored.

**Evidence.** The tests have stable names, so a requirements file can
list `VHDLTest_TestPasses` and `VHDLTest_TestFails` as the tests that
prove the tool works in its environment.
