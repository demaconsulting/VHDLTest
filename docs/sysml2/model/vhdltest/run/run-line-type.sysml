package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunLineType {
        doc /* Classification of an output line (for example info, warning, error). */

        metadata info : SoftwareInfo {
            description = "Classification of an output line (for example info, warning, error).";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunLineType.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunLineTypeTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-line-type.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-line-type.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-line-type.yaml");
        }
    }
}
