---
name: vhdltest
schema: 2
title: VHDLTest
summary: Built-in help of VHDLTest, which runs VHDL test benches with a simulator and saves the results.
---
{{tool}} compiles the VHDL files that a configuration file lists, runs each test bench with an installed simulator,
reports which benches passed or failed, and can save the results as a TRX or JUnit file.

Every operation is an option. The only command is `{{tool}} help`, which must be the first argument and takes topic
names, such as `{{tool}} help command-line`. Any other word that is not an option is the name of a test bench.
