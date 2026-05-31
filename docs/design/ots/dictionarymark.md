## DemaConsulting.DictionaryMark Integration Design

### Purpose

DemaConsulting.DictionaryMark provides dictionary generation, conflict-detection, and spelling
enforcement utilities. DictionaryMark's spelling enforcement capability is demonstrated through
its self-validation test suite, exercised via the `--validate` mode in CI. It is not invoked
directly against the project's documentation files. DictionaryMark is not deployed with VHDLTest.

### Features Used

- **Spelling enforcement**: validates documentation files against a project dictionary
  configuration, flagging words not present in the approved word list and failing the build
  on any spelling violation.
- **Bullet-list generation**: generates bullet-list format dictionary entries for
  embedding recognized terms in documentation.
- **Table generation**: generates table format dictionary entries for structured
  presentation of the approved word list.
- **Conflict detection**: detects conflicting word definitions within the dictionary
  configuration, flagging ambiguous or duplicate entries.

### Integration Pattern

DictionaryMark is available as a local dotnet tool (`.config/dotnet-tools.json`) and is
invoked in the `build-docs` job of `.github/workflows/build.yaml` with `--validate --results`
to run its built-in self-validation test suite. The self-validation exercises all of
DictionaryMark's capabilities — bullet-list generation, table generation, conflict detection,
and spelling enforcement — against built-in fixtures, and writes a TRX result file consumed
by ReqStream for requirements traceability:

```bash
dotnet dictionarymark --validate --results artifacts/dictionarymark-self-validation.trx
```
