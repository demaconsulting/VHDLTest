package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def SelfTestSubsystem {
        doc /* Self-validation test runner invoked via the --validate option. */

        metadata info : SoftwareInfo {
            description = "Self-validation test runner invoked via the --validate option.";
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/SelfTest/ValidationTests.cs");
            designFiles = ("docs/design/vhdltest/self-test.md");
            verificationFiles = ("docs/verification/vhdltest/self-test.md");
            requirementFiles = ("docs/reqstream/vhdltest/self-test/self-test.yaml");
        }

        part validation : Validation;
    }
}
