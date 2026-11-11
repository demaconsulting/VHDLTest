package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def GhdlSimulator {
        doc /* GHDL simulator integration. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/GhdlSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/GhdlSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/ghdl-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/ghdl-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/ghdl-simulator.yaml");
        }
    }
}
