---
title: GHDL
summary: Run the test benches with the open-source GHDL simulator.
tags:
  - reference
order: 1
see-also:
  - simulators/nvc
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-ghdl.sh
    description: Run the tests with GHDL and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator ghdl --results results.trx
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Use a GHDL that is not on PATH.
    content: |
      export VHDLTEST_GHDL_PATH=/opt/ghdl/bin
---
[GHDL](https://github.com/ghdl/ghdl) is an open-source VHDL simulator.
Select it with `--simulator ghdl` ([[#run]]).
It is the first simulator that the tool tries when `--simulator` is
not given.

**Finding it.** The tool runs `ghdl` from the folder in
`VHDLTEST_GHDL_PATH` ([[#set-path]]), or else from `PATH`.

**Commands.** In the folder of the configuration file, the tool
compiles all files with `ghdl -a --std=08` into the library `work` in
`VHDLTest.out/GHDL`, using the response file `compile.rsp` there. For
each bench it runs `ghdl -e` and then `ghdl -r` with the same options.
With the LLVM or GCC backend, `ghdl -e` links each bench into a program
named after the bench in lowercase, with `e~<bench>.o` beside it, in
the folder of the configuration file; add both to `.gitignore` (see
[[configuration]]). The mcode backend writes no such files.

**Results.** While compiling, a message with a file position is an
error unless it is a `warning:`, and so are `:error:` and
`cannot open` lines. While running a bench, `(assertion error)`,
`(report error)`, `(assertion failure)`, `(report failure)`, and
`:error:` lines are errors, `(assertion warning)` and
`(report warning)` lines are warnings, and `(assertion note)` and
`(report note)` lines are notes.

**Needs.** GHDL with VHDL-2008 support; any backend (mcode, LLVM, or
GCC) works. In GitHub Actions, install it with `ghdl/setup-ghdl`; on
Windows runners, set up MSYS2 first.
