package VHDLTest {
    public import VHDLTest_CliSubsystem::*;
    public import VHDLTest_Program::*;
    public import VHDLTest_ResultsSubsystem::*;
    public import VHDLTest_RunSubsystem::*;
    public import VHDLTest_SelfTestSubsystem::*;
    public import VHDLTest_SimulatorsSubsystem::*;
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;

    doc
    /*
     * VHDLTest is a .NET command-line tool that runs VHDL test benches and generates
     * standard test results files across multiple VHDL simulators.
     */

    part def VHDLTestSystem {
        doc /* The VHDLTest system as a whole. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/IntegrationTests.cs");
            designFiles = ("docs/design/vhdltest.md");
            verificationFiles = ("docs/verification/vhdltest.md");
            requirementFiles = ("docs/reqstream/vhdltest/vhdltest.yaml");
        }

        part program : Program;
        part cli : CliSubsystem;
        part simulators : SimulatorsSubsystem;
        part run : RunSubsystem;
        part results : ResultsSubsystem;
        part selfTest : SelfTestSubsystem;

        part yamlDotNet : YamlDotNet;
        part demaConsultingTestResults : DemaConsultingTestResults;
        part helpCards : HelpCards;
    }
}
