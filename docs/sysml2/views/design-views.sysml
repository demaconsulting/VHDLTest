// Views rendered into docs/design/generated/ for the Design document.
// Per SysML2Tools syntax, 'expose' is only valid inside a named 'view' usage
// (not a 'view def' definition), and it - not 'render' - is what scopes the
// rendered diagram content.
package VHDLTestDesignViews {
    private import StandardViewDefinitions::*;
    private import VHDLTest::*;

    view SoftwareStructureView : InterconnectionView {
        expose VHDLTest::VHDLTestSystem;
    }

    view VHDLTestView {
        expose VHDLTestSystem;
    }

    view CliView {
        expose CliSubsystem;
    }

    view SimulatorsView {
        expose SimulatorsSubsystem;
    }

    view RunView {
        expose RunSubsystem;
    }

    view ResultsView {
        expose ResultsSubsystem;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
    }
}
