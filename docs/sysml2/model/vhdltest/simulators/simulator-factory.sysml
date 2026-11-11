package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def SimulatorFactory {
        doc /* Creates simulator instances by name or by auto-discovery. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/SimulatorFactory.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/SimulatorFactoryTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/simulator-factory.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/simulator-factory.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/simulator-factory.yaml");
        }
    }
}
