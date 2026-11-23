package VHDLTest_RunSubsystem {
    public import VHDLTest_ProcessInvoker::*;
    public import VHDLTest_RunLineRule::*;
    public import VHDLTest_RunLineType::*;
    public import VHDLTest_RunLine::*;
    public import VHDLTest_RunProcessor::*;
    public import VHDLTest_RunProgram::*;
    public import VHDLTest_RunResults::*;
    private import SoftwareItemMetadata::*;
    part def RunSubsystem {
        doc /* Process execution abstraction and simulation output processing. */

        metadata info : SoftwareInfo {
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
