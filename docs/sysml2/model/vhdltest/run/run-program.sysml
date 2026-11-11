package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunProgram {
        doc /* Executes simulation programs and captures their output. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunProgram.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunProgramTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-program.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-program.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-program.yaml");
        }
    }
}
