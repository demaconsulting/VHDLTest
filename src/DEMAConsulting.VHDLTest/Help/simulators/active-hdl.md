---
title: Active-HDL
summary: Run the test benches with the Active-HDL simulator from Aldec.
aliases:
  - activehdl
  - aldec
tags:
  - reference
order: 5
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-activehdl.sh
    description: Run the tests with Active-HDL and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator activehdl --results results.trx
  - name: set-path
    kind: snippet
    language: powershell
    save-as: set-path.ps1
    description: Use an Active-HDL that is not on PATH.
    content: |
      $env:VHDLTEST_ACTIVEHDL_PATH = 'C:\Aldec\Active-HDL\BIN'
---
Active-HDL is a commercial HDL simulator from Aldec, also sold as a
Lattice Edition. Select it with `--simulator activehdl`
([[#run]]).

**Finding it.** The tool runs `vsimsa`, the command-line simulator of
Active-HDL, from the folder in `VHDLTEST_ACTIVEHDL_PATH`
([[#set-path]]), or else from `PATH`.

**Commands.** In the folder of the configuration file, the tool writes
`VHDLTest.out/ActiveHDL/compile.do` and runs `vsimsa -do` with it. The
script creates the library `work` with `alib` and compiles each file
with `acom -2008 -dbg`. For each bench it writes `test.do`, which loads
the bench with `asim` and runs it with `run -all`.

**Results.** While compiling, `Error:` and `RUNTIME: Fatal Error` lines
are errors and `KERNEL: Warning:` lines are warnings. While running a
bench, `EXECUTION:: ERROR`, `EXECUTION:: FAILURE`, `KERNEL: ERROR`,
`RUNTIME: Fatal Error:`, and `VSIM: Error:` lines are errors,
`KERNEL: Warning:` and `EXECUTION:: WARNING` lines are warnings, and
`EXECUTION:: NOTE` lines are notes; the license warnings of the Lattice
Edition are ignored.

**Needs.** An installed Active-HDL with `vsimsa` and VHDL-2008 support.
