## HelpCards Verification

This document provides the verification evidence for the HelpCards OTS software item.
Requirements for this OTS item are defined in the HelpCards OTS Software Requirements document.

### Required Functionality

HelpCards (`VHDLTest-OTS-HelpCards-Cli` and `VHDLTest-OTS-HelpCards-Testing`) shall answer the
help command of the tool from the cards embedded in the tool assembly, and report card defects and
example results to the tests of the tool.

### Verification Approach

HelpCards is a runtime library with no self-validation mode, so it is verified by integration test
evidence from the tests of the tool, which use the real package and the cards embedded in the
tool assembly.

### Test Environment

Standard .NET test runner environment — no additional setup required. The example runner runs the
built tool in a scratch folder; every command example that runs test benches uses the built-in
mock simulator, so no VHDL simulator is needed.

### Acceptance Criteria

The help tests in `HelpCommandTests.cs` and `IntegrationTests.cs` pass under `dotnet test` on all
supported platforms.

### Test Scenarios

**Help Command**: The help verb is answered from the embedded cards, in-process and by the built
tool. This scenario is tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero` and
`VHDLTest_HelpTopics_HelpVerb_ListsTopics`.

**Test Helpers**: The validator accepts the shipped cards, and the example runner runs every
command example against the built tool. This scenario is tested by
`HelpCommand_CreateOptions_ShippedCatalog_PassesValidation` and
`VHDLTest_HelpExamples_AllCommandExamples_GiveStatedResults`.
