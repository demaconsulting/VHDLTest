package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunResults {
        doc /* Aggregated results of a simulation run. */

        metadata info : SoftwareInfo {
            description = "Aggregated results of a simulation run.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunResults.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunResultsTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-results.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-results.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-results.yaml");
        }
    }
}
