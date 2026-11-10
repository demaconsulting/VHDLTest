package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunLine {
        doc /* An individual output line captured from a simulation run. */

        metadata info : SoftwareInfo {
            description = "An individual output line captured from a simulation run.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunLine.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunLineTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-line.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-line.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-line.yaml");
        }
    }
}
