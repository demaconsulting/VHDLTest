---
title: Vivado
summary: Run the test benches with the simulator of the AMD Vivado design suite.
aliases:
  - xsim
tags:
  - reference
order: 4
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-vivado.sh
    description: Run the tests with Vivado and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator vivado --results results.trx
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Use a Vivado that is not on PATH.
    content: |
      export VHDLTEST_VIVADO_PATH=/opt/Xilinx/Vivado/2024.2/bin
---
Vivado is the FPGA design suite of AMD (formerly Xilinx); its simulator,
XSim, runs from the `xvhdl` and `xelab` programs. Select it with
`--simulator vivado` ([[#run]]).

**Finding it.** The tool looks for `vivado` in the folder in
`VHDLTEST_VIVADO_PATH` ([[#set-path]]), or else on `PATH`.
It runs `xvhdl` and `xelab` from that same folder.

**Commands.** The tool writes the argument file `compile.do` to
`VHDLTest.out/Vivado` below the folder of the configuration file and
runs `xvhdl -file compile.do` there, which compiles each file as
VHDL-2008 into the library `work`. For each bench it writes `test.do`
and runs `xelab -file test.do`, which elaborates the bench and runs it
to the end (`-standalone -runall`).

**Results.** While compiling, `Error:` lines are errors. While running
a bench, `Error:` and `Failure:` lines are errors, `Warning:` lines are
warnings, and `Note:` lines are notes.

**Needs.** An installed Vivado; the folder of `vivado` must also hold
`xvhdl` and `xelab`. The argument files name each file as `../../<path>`
from `VHDLTest.out/Vivado`, so the paths in the configuration file must
be relative to that file, not absolute.
