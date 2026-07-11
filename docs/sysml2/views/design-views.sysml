// Views rendered into docs/design/generated/ for the Design document.
// Reopens the VHDLTest package so each 'expose' statement can reference
// its target part def directly. Per SysML2Tools syntax, 'expose' is only valid
// inside a named 'view' usage (not a 'view def' definition), and it - not
// 'render' - is what scopes the rendered diagram content.
package VHDLTest {
    view SoftwareStructureView {
        expose VHDLTest;
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
