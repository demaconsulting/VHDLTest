package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def Options {
        doc /* Parsed options holder. */

        metadata info : SoftwareInfo {
            description = "Parsed options holder.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Cli/Options.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/OptionsTests.cs");
            designFiles = ("docs/design/vhdltest/cli/options.md");
            verificationFiles = ("docs/verification/vhdltest/cli/options.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/options.yaml");
        }
    }
}
