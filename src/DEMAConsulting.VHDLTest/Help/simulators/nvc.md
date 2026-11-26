---
title: NVC
summary: Run the test benches with the open-source NVC simulator.
tags:
  - reference
order: 6
see-also:
  - simulators/ghdl
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-nvc.sh
    description: Run the tests with NVC and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator nvc --results results.trx
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Use an NVC that is not on PATH.
    content: |
      export VHDLTEST_NVC_PATH=/opt/nvc/bin
---
[NVC](https://www.nickg.me.uk/nvc/) is an open-source VHDL compiler and
simulator. Select it with `--simulator nvc` ([[#run]]).
Without `--simulator`, the tool tries it last.

**Finding it.** The tool runs `nvc` from the folder in
`VHDLTEST_NVC_PATH` ([[#set-path]]), or else from `PATH`.

**Commands.** In the folder of the configuration file, the tool
compiles all files with `nvc --std=2008 -a` into the library `work` in
`VHDLTest.out/NVC/lib`, using the response file `compile.rsp`. For each
bench it runs one `nvc` command that elaborates (`-e`) and runs (`-r`)
the bench.

**Results.** Lines with `Error:`, `Failure:`, or `Fatal:` are errors,
`Warning:` lines are warnings, and `Note:` lines are notes.

**Needs.** NVC with VHDL-2008 support. In GitHub Actions on Linux or
Windows, `nickg/setup-nvc-ci` installs it (on Windows after MSYS2); on
macOS use `brew install nvc`.
