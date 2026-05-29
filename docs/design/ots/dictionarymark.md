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
- **Bullet-list generation**: generates bullet-list format dictionary entries for
  embedding recognized terms in documentation.
- **Table generation**: generates table format dictionary entries for structured
  presentation of the approved word list.
- **Conflict detection**: detects conflicting word definitions within the dictionary
  configuration, flagging ambiguous or duplicate entries.

### Integration Pattern

DictionaryMark is available as a local dotnet tool (`.config/dotnet-tools.json`) and is
invoked in the `quality-checks` job of `.github/workflows/build.yaml` to enforce spelling
standards across design, requirements, and verification documentation before the
documentation is formally compiled and reviewed. It is invoked with default project
configuration (no explicit config file argument required):

```bash
dotnet dictionarymark
```
