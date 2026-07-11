package VHDLTest {
    part def ResultsSubsystem {
        doc /* Test result modeling and collection. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/Results/ResultsSubsystemTests.cs */
        comment designRef /* Design: docs/design/vhdltest/results.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/results.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/results/results.yaml */

        part testResult : TestResult;
        part testResults : TestResults;
    }
}
