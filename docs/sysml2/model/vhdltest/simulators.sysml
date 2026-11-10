package VHDLTest {
    private import SoftwareItemMetadata::*;
    part def SimulatorsSubsystem {
        doc /* VHDL simulator abstraction, discovery, and per-tool integrations. */

        metadata info : SoftwareInfo {
            description = "VHDL simulator abstraction, discovery, and per-tool integrations.";
            testFiles = ("test/DEMAConsulting.VHDLTest.Tests/Simulators/SimulatorsSubsystemTests.cs");
            designFiles = ("docs/design/vhdltest/simulators.md");
            verificationFiles = ("docs/verification/vhdltest/simulators.md");
            requirementFiles = ("docs/reqstream/vhdltest/simulators.yaml");
        }

        part simulator : Simulator;
        part simulatorFactory : SimulatorFactory;
        part ghdlSimulator : GhdlSimulator;
        part modelSimSimulator : ModelSimSimulator;
        part questaSimSimulator : QuestaSimSimulator;
        part vivadoSimulator : VivadoSimulator;
        part activeHdlSimulator : ActiveHdlSimulator;
        part nvcSimulator : NvcSimulator;
        part mockSimulator : MockSimulator;
        part tclText : TclText;
        part xilinxArgText : XilinxArgText;
    }
}
