---
title: Getting started
summary: Install the tool and a simulator, run test benches, and run them in CI.
aliases:
  - start
  - quickstart
  - examples
  - install
  - ci
  - github-actions
tags:
  - tutorial
order: 1
see-also:
  - command-line
  - configuration
  - simulators
  - test-results
examples:
  - name: install-local
    kind: snippet
    language: shell
    save-as: install-local.sh
    description: Add the tool to the tool manifest of a repository.
    content: |
      dotnet new tool-manifest
      dotnet tool install --local DemaConsulting.VHDLTest
  - name: ci-steps
    kind: snippet
    language: yaml
    save-as: ci-steps.yaml
    description: GitHub Actions steps that set up .NET and GHDL and run the tests.
    content: |
      - name: Check out the repository
        uses: actions/checkout@v4
      - name: Set up .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.x
      - name: Install GHDL
        uses: ghdl/setup-ghdl@v1
        with:
          backend: mcode
      - name: Restore the local .NET tools
        run: dotnet tool restore
      - name: Run the VHDL test benches
        run: dotnet {{tool}} --config tests.yaml --simulator ghdl --results test-results.trx
  - name: version
    kind: command
    language: shell
    description: Print the version and exit.
    content: '{{tool}} --version'
  - name: validate
    kind: command
    language: shell
    description: Run the self-tests with the built-in mock simulator.
    content: '{{tool}} --validate --simulator mock'
    expect: 'Validation Passed'
  - name: first-run
    kind: command
    language: shell
    description: Run the sample tests.yaml with the mock simulator. Exits 1.
    content: '{{tool}} --config tests.yaml --simulator mock'
    exit: 1
    expect: 'Failed 2 of 4 tests'
  - name: real-run
    kind: snippet
    language: shell
    save-as: run-ghdl.sh
    description: Run the same tests with GHDL and save the results.
    content: |
      {{tool}} --config tests.yaml --simulator ghdl --results test-results.trx
  - name: topic
    kind: command
    language: shell
    description: Print a help topic, here the list of options.
    content: '{{tool}} help command-line'
    expect: '--simulator'
---
{{tool}} {{version}} is a .NET tool.
It compiles the VHDL files that a configuration file lists, runs each
test bench with a simulator, and exits 1 when a bench fails. It writes
the files that options name, the simulator files in a `VHDLTest.out`
folder beside the configuration file, and, during `--validate`, a
`validation.tmp` folder that it removes afterwards. Some simulators
also leave files beside the configuration file; [[configuration]] lists
them and the `.gitignore` entries to add.

1. **Install it.** The tool needs the .NET 10 SDK or later. For a
   repository or CI, install it as a local tool. Run the commands
   that [[#install-local]] prints once, without
   `dotnet new tool-manifest` when `.config/dotnet-tools.json` exists, and
   commit that file. For one machine, install it globally and run it
   as `{{tool}}`:

   ```bash
   dotnet tool install -g DemaConsulting.VHDLTest
   ```

2. **Install a simulator.** {{tool}} drives GHDL, NVC, ModelSim,
   QuestaSim, Vivado, or Active-HDL; it does not include one. See
   [[simulators]].
3. **Check it.** The command that [[#version]] prints shows the version.
   The command from [[#validate]] runs the built-in self-tests with the
   mock simulator, which needs no installation; see [[self-validation]].
4. **Write a configuration.** A YAML file lists the VHDL files and the
   test benches; see [[configuration]], which has a complete sample.
   The command from [[#first-run]] runs that sample with the mock
   simulator: two benches pass and two fail, so it exits 1. With a real
   simulator installed, run it as [[#real-run]] shows.
5. **Run it in CI.** In GitHub Actions, copy the steps that
   [[#ci-steps]] prints. They set up .NET 10 with
   `actions/setup-dotnet`, install GHDL, run `dotnet tool restore`,
   and then run `dotnet {{tool}}`. A failing bench exits 1 and fails
   the step. On Windows runners, GHDL needs MSYS2 set up first.

Print any help topic with `{{tool}} help <topic>`, as in the command that
[[#topic]] prints. Add `--search <words>` to find a topic, and `--json` to
get any help answer as JSON.
