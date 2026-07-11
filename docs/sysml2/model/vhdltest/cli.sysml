package VHDLTest {
    part def CliSubsystem {
        doc /* Command-line argument parsing, configuration loading, and options holder. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/Cli/CliSubsystemTests.cs */
        comment designRef /* Design: docs/design/vhdltest/cli.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/cli.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/cli/cli.yaml */

        part context : Context;
        part configDocument : ConfigDocument;
        part options : Options;
    }
}
