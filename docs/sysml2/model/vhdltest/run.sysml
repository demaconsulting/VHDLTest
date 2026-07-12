package VHDLTest {
    part def RunSubsystem {
        doc /* Process execution abstraction and simulation output processing. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/Run/RunSubsystemTests.cs */
        comment designRef /* Design: docs/design/vhdltest/run.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/run.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/run/run.yaml */

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
