package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def TclText {
        doc /* Quotes string values for safe interpolation into generated TCL do-scripts. */

        metadata info : SoftwareInfo {
            description = "Quotes string values for safe interpolation into generated TCL do-scripts.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/TclText.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/TclTextTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/tcl-text.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/tcl-text.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/tcl-text.yaml");
        }
    }
}
