package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def TestResults {
        doc /* A collection of test results. */

        metadata info : SoftwareInfo {
            description = "A collection of test results.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Results/TestResults.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Results/TestResultsTests.cs");
            designFiles = ("docs/design/vhdltest/results/test-results.md");
            verificationFiles = ("docs/verification/vhdltest/results/test-results.md");
            requirementFiles = ("docs/reqstream/vhdltest/results/test-results.yaml");
        }
    }
}
