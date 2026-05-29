## DemaConsulting.DictionaryMark Verification

### Verification Approach

DemaConsulting.DictionaryMark is verified through its self-validation mode. The `build-docs`
job in `.github/workflows/build.yaml` invokes DictionaryMark with `--validate` to execute
its built-in test suite. A passing self-validation TRX result constitutes evidence that
DictionaryMark's dictionary management and enforcement capabilities are functioning correctly.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Ubuntu (quality-checks job). The
project custom dictionary configuration must be present in the repository.

### Acceptance Criteria

The DictionaryMark self-validation step in the CI pipeline completes with exit code 0 and
all self-validation tests pass. A passing result constitutes evidence that DictionaryMark's
dictionary generation, conflict detection, and enforcement capabilities are functioning correctly.

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
