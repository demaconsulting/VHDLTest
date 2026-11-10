package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def RunSubsystem {
        doc /* Process execution abstraction and simulation output processing. */

        metadata info : SoftwareInfo {
            description = "Process execution abstraction and simulation output processing.";
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Run/RunSubsystemTests.cs");
            designFiles = ("docs/design/vhdltest/run.md");
            verificationFiles = ("docs/verification/vhdltest/run.md");
            requirementFiles = ("docs/reqstream/vhdltest/run/run.yaml");
        }

        part iProcessInvoker : IProcessInvoker;
        part processInvoker : ProcessInvoker;
        part runProcessor : RunProcessor;
        part runProgram : RunProgram;
        part runResults : RunResults;
        part runLine : RunLine;
        part runLineRule : RunLineRule;
        part runLineType : RunLineType;
    }
}
