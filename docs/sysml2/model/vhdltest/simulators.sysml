package VHDLTest {
    part def SimulatorsSubsystem {
        doc /* VHDL simulator abstraction, discovery, and per-tool integrations. */

        comment testRef /* Test: test/DEMAConsulting.VHDLTest.Tests/Simulators/SimulatorsSubsystemTests.cs */
        comment designRef /* Design: docs/design/vhdltest/simulators.md */
        comment verificationRef /* Verification: docs/verification/vhdltest/simulators.md */
        comment reqRef /* Requirements: docs/reqstream/vhdltest/simulators.yaml */

        part simulator : Simulator;
        part simulatorFactory : SimulatorFactory;
        part ghdlSimulator : GhdlSimulator;
        part modelSimSimulator : ModelSimSimulator;
        part questaSimSimulator : QuestaSimSimulator;
        part vivadoSimulator : VivadoSimulator;
        part activeHdlSimulator : ActiveHdlSimulator;
        part nvcSimulator : NvcSimulator;
        part mockSimulator : MockSimulator;
    }
}
