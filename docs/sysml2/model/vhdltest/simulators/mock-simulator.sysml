package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def MockSimulator {
        doc /* Mock simulator used for self-validation; excluded from auto-discovery. */

        metadata info : SoftwareInfo {
            description = "Mock simulator used for self-validation; excluded from auto-discovery.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/MockSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/MockSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/mock-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/mock-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/mock-simulator.yaml");
        }
    }
}
