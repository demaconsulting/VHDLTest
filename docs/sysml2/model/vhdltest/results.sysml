package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def ResultsSubsystem {
        doc /* Test result modeling and collection. */

        metadata info : SoftwareInfo {
            description = "Test result modeling and collection.";
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Results/ResultsSubsystemTests.cs");
            designFiles = ("docs/design/vhdltest/results.md");
            verificationFiles = ("docs/verification/vhdltest/results.md");
            requirementFiles = ("docs/reqstream/vhdltest/results/results.yaml");
        }

        part testResult : TestResult;
        part testResults : TestResults;
    }
}
