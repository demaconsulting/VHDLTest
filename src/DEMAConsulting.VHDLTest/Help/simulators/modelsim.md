---
title: ModelSim
summary: Run the test benches with the ModelSim simulator from Siemens.
tags:
  - reference
order: 2
see-also:
  - simulators/questasim
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-modelsim.sh
    description: Run the tests with ModelSim and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator modelsim --results results.trx
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Use a ModelSim that is not on PATH.
    content: |
      export VHDLTEST_MODELSIM_PATH=/opt/modelsim/bin
---
ModelSim is a commercial HDL simulator from Siemens. Select it with
`--simulator modelsim` ([[#run]]).

**Finding it.** The tool runs `vsim` from the folder in
`VHDLTEST_MODELSIM_PATH` ([[#set-path]]), or else from `PATH`.
QuestaSim also provides `vsim`; without `--simulator`, a `vsim` on
`PATH` is used as ModelSim.

**Commands.** The tool writes the script `compile.do` to
`VHDLTest.out/ModelSim` below the folder of the configuration file and
runs `vsim -c -do compile.do` there. The script creates the library
`work` with `vlib` and compiles each file with `vcom -2008`. For each
bench it writes `test.do`, which loads the bench with `vsim` and runs it
with `run -all`.

**Results.** While compiling, `Error:` lines are errors. While running
a bench, `Error:` and `Failure:` lines are errors, `Warning:` lines are
warnings, and `Note:` lines are notes.

**Needs.** A licensed ModelSim with VHDL-2008 support. The scripts name
each file as `../../<path>` from `VHDLTest.out/ModelSim`, so the paths
in the configuration file must be relative to that file, not absolute.
