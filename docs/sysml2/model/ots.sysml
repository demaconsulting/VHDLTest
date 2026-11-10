package OtsDependencies {
    private import SoftwareItemMetadata::*;
    doc /* Off-the-shelf (OTS) dependencies used by VHDLTest. */

    part def YamlDotNet {
        doc /* OTS: YamlDotNet. */

        metadata info : SoftwareInfo {
            description = "OTS: YamlDotNet.";
            designFiles = ("docs/design/ots/yamldotnet.md");
            verificationFiles = ("docs/verification/ots/yamldotnet.md");
            requirementFiles = ("docs/reqstream/ots/yamldotnet.yaml");
        }
    }

    part def DemaConsultingTestResults {
        doc /* OTS: DemaConsulting.TestResults. */

        metadata info : SoftwareInfo {
            description = "OTS: DemaConsulting.TestResults.";
            designFiles = ("docs/design/ots/demaconsulting-testresults.md");
            verificationFiles = ("docs/verification/ots/demaconsulting-testresults.md");
            requirementFiles = ("docs/reqstream/ots/demaconsulting-testresults.yaml");
        }
    }
}
