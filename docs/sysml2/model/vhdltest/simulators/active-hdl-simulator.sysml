package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def ActiveHdlSimulator {
        doc /* Active-HDL simulator integration. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/ActiveHdlSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/ActiveHdlSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/active-hdl-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/active-hdl-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/active-hdl-simulator.yaml");
        }
    }
}
