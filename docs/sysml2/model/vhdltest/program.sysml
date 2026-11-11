package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def Program {
        doc /* Entry point and execution orchestrator for VHDLTest. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Program.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/ProgramTests.cs");
            designFiles = ("docs/design/vhdltest/program.md");
            verificationFiles = ("docs/verification/vhdltest/program.md");
            requirementFiles = ("docs/reqstream/vhdltest/program.yaml");
        }
    }
}
