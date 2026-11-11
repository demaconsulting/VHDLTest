package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def NvcSimulator {
        doc /* NVC simulator integration. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/NvcSimulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/NvcSimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/nvc-simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/nvc-simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/nvc-simulator.yaml");
        }
    }
}
