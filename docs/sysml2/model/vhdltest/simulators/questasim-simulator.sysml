package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def QuestaSimSimulator {
        doc /* QuestaSim simulator integration. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/QuestaSimSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/QuestaSimSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/questasim-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/questasim-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/questasim-simulator.yaml");
        }
    }
}
