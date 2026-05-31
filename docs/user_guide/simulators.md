# Supported Simulators

VHDLTest supports the following VHDL simulators:

## Active-HDL

[Active-HDL](https://www.aldec.com/en/products/fpga_simulation/active-hdl) is a commercial HDL simulator from Aldec.

**Configuration**: Set the `VHDLTEST_ACTIVEHDL_PATH` environment variable to the Active-HDL installation folder.

## GHDL

[GHDL](https://github.com/ghdl/ghdl) is an open-source VHDL simulator.

**Configuration**: Set the `VHDLTEST_GHDL_PATH` environment variable to the GHDL installation folder if not in PATH.

## ModelSim

[ModelSim](https://eda.sw.siemens.com/en-US/ic/modelsim/) is a commercial HDL simulator from Siemens.

**Configuration**: Set the `VHDLTEST_MODELSIM_PATH` environment variable to the ModelSim installation folder.

## NVC

[NVC](https://www.nickg.me.uk/nvc) is an open-source VHDL simulator and compiler.

**Configuration**: Set the `VHDLTEST_NVC_PATH` environment variable to the NVC installation folder if not in PATH.

## QuestaSim

[QuestaSim](https://eda.sw.siemens.com/en-US/ic/questa-one/simulation/) is an advanced verification platform from
Siemens, offering enhanced performance and capabilities compared to ModelSim.

**Configuration**: Set the `VHDLTEST_QUESTASIM_PATH` environment variable to the QuestaSim installation folder.

## Vivado

[Vivado](https://www.xilinx.com/products/design-tools/vivado.html) is Xilinx's design suite for FPGAs.

**Configuration**: Set the `VHDLTEST_VIVADO_PATH` environment variable to the Vivado installation folder.

## Mock

The mock simulator is a built-in simulator used for self-validation. It does not require any external
installation and is available on all platforms.

**Configuration**: No installation or environment variable is required. Specify `--simulator mock` to
use the mock simulator explicitly (for example, during `--validate` self-validation runs).

## Automatic Simulator Selection

When `--simulator` is omitted, VHDLTest automatically probes for installed simulators in the
following order and selects the first one found:

1. GHDL
2. ModelSim
3. QuestaSim
4. Vivado
5. Active-HDL
6. NVC

The `mock` simulator is never auto-selected and must always be specified explicitly via
`--simulator mock`.
