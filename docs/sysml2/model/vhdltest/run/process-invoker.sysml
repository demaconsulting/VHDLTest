package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def IProcessInvoker {
        doc /* Contract for launching an external process and returning its exit code and output. */

        metadata info : SoftwareInfo {
            description = "Contract for launching an external process and returning its exit code and output.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/IProcessInvoker.cs");
            designFiles = ("docs/design/vhdltest/run/process-invoker.md");
            verificationFiles = ("docs/verification/vhdltest/run/process-invoker.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/process-invoker.yaml");
        }
    }

    part def ProcessInvoker {
        doc /* Production implementation of IProcessInvoker delegating to RunProgram. */

        metadata info : SoftwareInfo {
            description = "Production implementation of IProcessInvoker delegating to RunProgram.";
            sourceFiles = ("src/DEMAConsulting.VHDLTest/Run/ProcessInvoker.cs");
            designFiles = ("docs/design/vhdltest/run/process-invoker.md");
            verificationFiles = ("docs/verification/vhdltest/run/process-invoker.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/process-invoker.yaml");
        }
    }
}
