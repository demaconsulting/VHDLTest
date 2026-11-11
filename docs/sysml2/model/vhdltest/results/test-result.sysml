package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def TestResult {
        doc /* An individual test result. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Results/TestResult.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Results/TestResultTests.cs");
            designFiles = ("docs/design/vhdltest/results/test-result.md");
            verificationFiles = ("docs/verification/vhdltest/results/test-result.md");
            requirementFiles = ("docs/reqstream/vhdltest/results/test-result.yaml");
        }
    }
}
