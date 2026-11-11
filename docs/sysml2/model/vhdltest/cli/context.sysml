package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def Context {
        doc /* Command-line argument parser and I/O owner. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Cli/Context.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Cli/ContextTests.cs");
            designFiles = ("docs/design/vhdltest/cli/context.md");
            verificationFiles = ("docs/verification/vhdltest/cli/context.md");
            requirementFiles = ("docs/reqstream/vhdltest/cli/context.yaml");
        }
    }
}
