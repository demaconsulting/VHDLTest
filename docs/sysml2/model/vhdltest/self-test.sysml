package VHDLTest {
    part def SelfTestSubsystem {
        doc /* Self-validation test runner invoked via the --validate option. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/SelfTest/ValidationTests.cs */
        comment designRef /* Design: docs/design/vhdltest/self-test.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/self-test.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/self-test/self-test.yaml */

        part validation : Validation;
    }
}
