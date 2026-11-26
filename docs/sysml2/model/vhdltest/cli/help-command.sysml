package VHDLTest_HelpCommand {
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;

    part def HelpCommand {
        doc /* Built-in help command: answers the help verb from the help cards embedded in the assembly. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Cli/HelpCommand.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/HelpCommandTests.cs");
            designFiles = ("docs/design/vhdltest/cli/help-command.md");
            verificationFiles = ("docs/verification/vhdltest/cli/help-command.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/help-command.yaml");
        }
    }

    // Serves the help command from the cards embedded in the assembly
    dependency HelpCommand to HelpCards;
}
