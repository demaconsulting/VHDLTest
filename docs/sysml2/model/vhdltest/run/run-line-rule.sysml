package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunLineRule {
        doc /* A rule matching an output line to classify it by RunLineType. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunLineRule.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunLineRuleTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-line-rule.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-line-rule.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-line-rule.yaml");
        }
    }
}
