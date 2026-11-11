package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def Validation {
        doc /* Runs the embedded self-validation test scenarios via the Mock simulator. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/SelfTest/Validation.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/SelfTest/ValidationTests.cs");
            designFiles = ("docs/design/vhdltest/self-test/validation.md");
            verificationFiles = ("docs/verification/vhdltest/self-test/validation.md");
            requirementFiles = ("docs/reqstream/vhdltest/self-test/validation.yaml");
        }
    }
}
