package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def Simulator {
        doc /* Base class for all VHDL simulator integrations. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/Simulator.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/SimulatorTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/simulator.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/simulator.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/simulator.yaml");
        }
    }
}
