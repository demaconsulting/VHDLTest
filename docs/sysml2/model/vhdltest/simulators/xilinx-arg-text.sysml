package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def XilinxArgText {
        doc /* Quotes string values for safe interpolation into generated Xilinx argument files. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Simulators/XilinxArgText.cs");
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/XilinxArgTextTests.cs");
            designFiles = ("docs/design/vhdltest/simulators/xilinx-arg-text.md");
            verificationFiles = ("docs/verification/vhdltest/simulators/xilinx-arg-text.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators/xilinx-arg-text.yaml");
        }
    }
}
