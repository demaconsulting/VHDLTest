package VHDLTest_CliSubsystem {
    public import VHDLTest_ConfigDocument::*;
    public import VHDLTest_Context::*;
    public import VHDLTest_Options::*;
    public import VHDLTest_HelpCommand::*;
    private import SoftwareItemMetadata::*;
    part def CliSubsystem {
        doc /* Command-line argument parsing, configuration loading, options holder, and the built-in help command. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/CliSubsystemTests.cs");
            designFiles = ("docs/design/vhdltest/cli.md");
            verificationFiles = ("docs/verification/vhdltest/cli.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/cli.yaml");
        }

        part context : Context;
        part configDocument : ConfigDocument;
        part options : Options;
        part helpCommand : HelpCommand;
    }
}
