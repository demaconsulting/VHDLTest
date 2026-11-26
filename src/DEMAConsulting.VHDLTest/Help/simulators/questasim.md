---
title: QuestaSim
summary: Run the test benches with the QuestaSim simulator from Siemens.
aliases:
  - questa
tags:
  - reference
order: 3
see-also:
  - simulators/modelsim
examples:
  - name: run
    kind: snippet
    language: shell
    save-as: run-questasim.sh
    description: Run the tests with QuestaSim and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator questasim --results results.trx
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Use a QuestaSim that is not on PATH.
    content: |
      export VHDLTEST_QUESTASIM_PATH=/opt/questasim/bin
---
QuestaSim, also called Questa, is a commercial HDL simulator from
Siemens. Select it with `--simulator questasim` ([[#run]]).

**Finding it.** The tool runs `vsim` from the folder in
`VHDLTEST_QUESTASIM_PATH` ([[#set-path]]), or else from `PATH`.
ModelSim also provides `vsim`, and ModelSim comes first when the tool
chooses a simulator itself, so always name `questasim`.

**Commands.** The tool writes the script `compile.do` to
`VHDLTest.out/QuestaSim` below the folder of the configuration file and
runs `vsim -c -do compile.do` there. The script creates the library
`work` with `vlib` and compiles each file with `vcom -2008`. For each
bench it writes `test.do`, which loads the bench with `vsim` and runs it
with `run -all`.

**Results.** While compiling, `Error:` lines are errors. While running
a bench, `Error:` and `Failure:` lines are errors, `Warning:` lines are
warnings, and `Note:` lines are notes.

**Needs.** A licensed QuestaSim with VHDL-2008 support. The scripts
name each file as `../../<path>` from `VHDLTest.out/QuestaSim`, so the
paths in the configuration file must be relative to that file, not
absolute.
