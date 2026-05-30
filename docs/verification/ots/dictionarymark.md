## DemaConsulting.DictionaryMark Verification

### Verification Approach

DemaConsulting.DictionaryMark is verified through its self-validation mode and through
direct enforcement in the CI pipeline. The `build-docs` job in
`.github/workflows/build.yaml` invokes DictionaryMark with `--validate` to execute its
built-in test suite and writes a TRX result consumed by ReqStream. A passing
self-validation TRX result constitutes evidence that DictionaryMark's dictionary
management capabilities are functioning correctly. Spell-checking enforcement (without
`--validate`) runs separately in the `quality-checks` job via `lint.ps1` and verifies
that project documentation is free of spelling violations.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Ubuntu (quality-checks job). The
project custom dictionary configuration must be present in the repository.

### Acceptance Criteria

The DictionaryMark self-validation step in the CI pipeline completes with exit code 0 and
all self-validation tests pass. The enforcement invocation also completes with exit code 0,
confirming that all documentation files comply with the configured dictionary. A passing
result constitutes evidence that DictionaryMark's dictionary generation, conflict
detection, and enforcement capabilities are functioning correctly.

### Test Scenarios

- **Bullet-list generation**: `DictionaryMark_BulletGeneration` verifies that DictionaryMark
  generates correctly formatted bullet-list entries from the configured dictionary, confirming
  the generation capability is operational.
- **Table generation**: `DictionaryMark_TableGeneration` verifies that DictionaryMark generates
  correctly formatted table entries from the configured dictionary, confirming the tabular
  generation output is correct.
- **Conflict detection**: `DictionaryMark_ConflictDetection` verifies that DictionaryMark
  correctly identifies conflicting word definitions within a dictionary configuration, confirming
  the validation logic detects ambiguous entries.
- **Spelling enforcement**: `DictionaryMark_SpellingEnforcement` verifies that DictionaryMark
  correctly enforces the configured dictionary against project documentation files in the
  quality-checks CI job, confirming that the enforcement capability rejects unrecognized words
  and passes when all documentation complies with the dictionary.
