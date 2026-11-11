package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def CliSubsystem {
        doc /* Command-line argument parsing, configuration loading, and options holder. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/CliSubsystemTests.cs");
            designFiles = ("docs/design/vhdltest/cli.md");
            verificationFiles = ("docs/verification/vhdltest/cli.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/cli.yaml");
        }

        part context : Context;
        part configDocument : ConfigDocument;
        part options : Options;
    }
}
