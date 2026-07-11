package VHDLTest {
    part def IProcessInvoker {
        doc /* Contract for launching an external process and returning its exit code and output. */

        comment sourceRef /* Source: src/DEMAConsulting.VHDLTest/Run/IProcessInvoker.cs */
        comment designRef /* Design: docs/design/vhdltest/run/process-invoker.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/run/process-invoker.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/run/process-invoker.yaml */
    }

    part def ProcessInvoker {
        doc /* Production implementation of IProcessInvoker delegating to RunProgram. */

        comment sourceRef /* Source: src/DEMAConsulting.VHDLTest/Run/ProcessInvoker.cs */
        comment designRef /* Design: docs/design/vhdltest/run/process-invoker.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/run/process-invoker.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/run/process-invoker.yaml */
    }
}
