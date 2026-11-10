package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def ConfigDocument {
        doc /* YAML configuration document parser. */

        metadata info : SoftwareInfo {
            description = "YAML configuration document parser.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Cli/ConfigDocument.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/ConfigDocumentTests.cs");
            designFiles = ("docs/design/vhdltest/cli/config-document.md");
            verificationFiles = ("docs/verification/vhdltest/cli/config-document.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/config-document.yaml");
        }
    }
}
