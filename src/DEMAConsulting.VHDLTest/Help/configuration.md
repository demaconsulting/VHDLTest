---
title: Configuration file
summary: The YAML file that lists the VHDL files to compile and the test benches to run.
aliases:
  - config
  - config-file
  - tests-file
  - yaml
tags:
  - reference
order: 3
see-also:
  - command-line
  - simulators
examples:
  - name: tests
    kind: source
    language: yaml
    save-as: tests.yaml
    description: A complete configuration with two designs and four test benches.
    content: |
      ---
      # VHDLTest configuration: the VHDL files to compile and the test benches
      # to run. Paths are relative to this file.

      # VHDL files, compiled in this order: put each file after the files
      # that it uses.
      files:
        - half_adder.vhd
        - full_adder.vhd
        - half_adder_pass_tb.vhd
        - half_adder_fail_tb.vhd
        - full_adder_pass_tb.vhd
        - full_adder_fail_tb.vhd

      # Test-bench entities to run, in this order.
      tests:
        - half_adder_pass_tb
        - half_adder_fail_tb
        - full_adder_pass_tb
        - full_adder_fail_tb
  - name: run
    kind: command
    language: shell
    description: Run the sample with the mock simulator. Two benches fail; exits 1.
    content: '{{tool}} --config tests.yaml --simulator mock'
    exit: 1
    expect: 'Passed 2 of 4 tests'
---
`--config <file>` names a YAML file with two keys. The sample that
[[#tests]] prints is a complete file.

| Key | Meaning |
| --- | --- |
| `files` | The VHDL files to compile, in order. |
| `tests` | The test-bench entities to run, in order. |

**Files.** All files are compiled as VHDL-2008 into the library `work`,
in the order listed, so put each file after the files that it uses.
Write the paths relative to the configuration file. A compile error
stops the run with `Error: Build Failed` before any bench runs.

**Tests.** Each entry is the name of a test-bench entity. A bench fails
when the simulator reports an error or failure, or exits with an error;
see [[simulators]]. Test-bench names on the command line replace this
list.

**Rules.** Both keys are optional; a missing key is an empty list. Any
other key, such as `test:` instead of `tests:`, or text that is not
valid YAML, is the error `Configuration document <file> is invalid`.

**Folders.** The tool keeps the libraries and scripts of the simulator
in a `VHDLTest.out` folder beside the configuration file. GHDL, NVC,
and Active-HDL run in the folder of the configuration file; ModelSim,
QuestaSim, and Vivado run in their folder below `VHDLTest.out`. Add
`VHDLTest.out` to `.gitignore`.

The command that [[#run]] prints runs the sample with the mock
simulator, which passes or fails a bench by its name. As with a real
simulator and these benches, two pass and two fail.
