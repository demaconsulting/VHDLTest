package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunProcessor {
        doc /* Processes simulation run output against configured line rules. */

        metadata info : SoftwareInfo {
            description = "Processes simulation run output against configured line rules.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/RunProcessor.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunProcessorTests.cs");
            designFiles = ("docs/design/vhdltest/run/run-processor.md");
            verificationFiles = ("docs/verification/vhdltest/run/run-processor.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run-processor.yaml");
        }
    }
}
