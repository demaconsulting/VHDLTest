---
title: Simulators
summary: The supported simulators, how one is found and chosen, and how a run is judged.
aliases:
  - simulator
  - sim
  - mock
  - simulator-path
  - ghdl
  - nvc
  - modelsim
  - questasim
  - questa
  - vivado
  - xsim
  - activehdl
  - active-hdl
tags:
  - reference
order: 4
see-also:
  - configuration
  - troubleshooting
examples:
  - name: mock-run
    kind: command
    language: shell
    description: Run the sample with the built-in mock simulator. Exits 1.
    content: '{{tool}} --config tests.yaml --simulator mock'
    exit: 1
    expect: 'Building with Mock...'
  - name: unknown
    kind: command
    language: shell
    description: A simulator name that does not exist. Exits 1.
    content: '{{tool}} --config tests.yaml --simulator unknown'
    exit: 1
    expect: 'Error: Simulator not found'
  - name: set-path
    kind: snippet
    language: shell
    save-as: set-path.sh
    description: Point the tool at the folder that holds the ghdl program.
    content: |
      export VHDLTEST_GHDL_PATH=/opt/ghdl/bin
---
{{tool}} does not include a simulator; it drives one that is installed.
Choose it with `--simulator <name>`; the name ignores case.

| Simulator | `--simulator` | Program | Folder variable |
| --- | --- | --- | --- |
| GHDL | `ghdl` | `ghdl` | `VHDLTEST_GHDL_PATH` |
| ModelSim | `modelsim` | `vsim` | `VHDLTEST_MODELSIM_PATH` |
| QuestaSim | `questasim` | `vsim` | `VHDLTEST_QUESTASIM_PATH` |
| Vivado | `vivado` | `vivado` | `VHDLTEST_VIVADO_PATH` |
| Active-HDL | `activehdl` | `vsimsa` | `VHDLTEST_ACTIVEHDL_PATH` |
| NVC | `nvc` | `nvc` | `VHDLTEST_NVC_PATH` |
| Mock | `mock` | built in | none |

**Finding a simulator.** A set folder variable is used as is, without a
check: set it to the folder that holds the program ([[#set-path]]).
Otherwise the tool searches `PATH`, on Windows after the current folder.

**Choosing a simulator.** Without `--simulator`, the tool takes the
first one it finds in the order of the table, from GHDL to NVC, so a
`vsim` on `PATH` selects ModelSim; name `questasim` to use QuestaSim.
An unknown name, or no simulator found, is `Error: Simulator not found`
([[#unknown]]). A named simulator that is not installed is
`Error: <Name> Simulator not available`. Both exit 1.

**Judging a run.** The tool first compiles every file. Any compile
error is `Error: Build Failed`, which exits 1 even with `--exit-0`, and
no bench runs. Then each bench runs on its own. A bench fails when the
simulator exits with an error code or prints an error: a VHDL
assertion or report of severity error or failure, or an error or fatal
message of the simulator. Notes and warnings are printed but do not
fail a bench. `--verbose` also shows the commands and all other output.

**Mock.** The mock simulator needs no installation and is never chosen
automatically ([[#mock-run]]). It does not read the VHDL files: a bench
whose name contains `_fail_` fails, a file or bench name that contains
`_error_` is an error, and every other bench passes. Use it to try the
tool and for `--validate`.

Each simulator except Mock has its own topic,
`{{tool}} help simulators <name>`, such as
`{{tool}} help simulators ghdl`. A simulator name alone, as in
`{{tool}} help ghdl`, opens this topic.
