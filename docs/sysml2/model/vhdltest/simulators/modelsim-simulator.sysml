package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def ModelSimSimulator {
        doc /* ModelSim simulator integration. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/ModelSimSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/ModelSimSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/modelsim-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/modelsim-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/modelsim-simulator.yaml");
        }
    }
}
