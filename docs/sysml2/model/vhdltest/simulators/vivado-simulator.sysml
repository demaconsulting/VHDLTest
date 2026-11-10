package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def VivadoSimulator {
        doc /* Vivado simulator integration. */

        metadata info : SoftwareInfo {
            description = "Vivado simulator integration.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/VivadoSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/VivadoSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/vivado-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/vivado-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/vivado-simulator.yaml");
        }
    }
}
