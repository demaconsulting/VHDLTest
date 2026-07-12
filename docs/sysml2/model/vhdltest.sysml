package VHDLTest {
    import OtsDependencies::*;

    doc
    /*
     * VHDLTest is a .NET command-line tool that runs VHDL test benches and generates
     * standard test results files across multiple VHDL simulators.
     */

    part def VHDLTestSystem {
        doc /* The VHDLTest system as a whole. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/IntegrationTests.cs */
        comment designRef /* Design: docs/design/vhdltest.md */
        comment verificationRef /* Verification: docs/verification/vhdltest.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/vhdltest.yaml */

        part program : Program;
        part cli : CliSubsystem;
        part simulators : SimulatorsSubsystem;
        part run : RunSubsystem;
        part results : ResultsSubsystem;
        part selfTest : SelfTestSubsystem;

        part yamlDotNet : YamlDotNet;
        part demaConsultingTestResults : DemaConsultingTestResults;
    }
}
