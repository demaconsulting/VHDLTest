## DemaConsulting.DictionaryMark Integration Design

### Purpose

DemaConsulting.DictionaryMark enforces correct spelling across project documentation
using a configured custom dictionary. It checks documentation files against the
project-defined word list and fails the build when unrecognized words are encountered,
maintaining consistent terminology and preventing typographical errors in reviewed
documents. DictionaryMark is not deployed with VHDLTest.

### Features Used

- **Custom dictionary enforcement**: validates documentation files against a project
  dictionary configuration, flagging words not present in the approved word list.
- **Build gate**: exits non-zero on any spelling violation, making unrecognized words
  a build-breaking condition.

### Integration Pattern

DictionaryMark is available as a local dotnet tool (`.config/dotnet-tools.json`) and is
invoked in the `quality-checks` job of `.github/workflows/build.yaml` to enforce spelling
standards across design, requirements, and verification documentation before the
documentation is formally compiled and reviewed. It is invoked with default project
configuration (no explicit config file argument required):

```bash
dotnet dictionarymark
```
