## DemaConsulting.DictionaryMark Verification

### Verification Approach

DemaConsulting.DictionaryMark is verified through CI pipeline execution. The quality-checks
job in `.github/workflows/build.yaml` invokes DictionaryMark to check all documentation
files against the project custom dictionary. A passing CI step constitutes evidence that
DictionaryMark executed correctly and all documentation files passed the spelling check.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Ubuntu (quality-checks job). The
project custom dictionary configuration must be present in the repository.

### Acceptance Criteria

The DictionaryMark step in the quality-checks CI job completes with exit code 0. A
passing step constitutes evidence that all checked documentation files contain only
recognized words from the project dictionary.

### Test Scenarios

- **Spell check passes on all documentation**: `dotnet dictionarymark` scans design,
  requirements, and verification documentation files. A zero exit code confirms that
  no unrecognized words were found in any checked file.
- **Build failure on spelling violation**: DictionaryMark exits non-zero when an
  unrecognized word is encountered, causing the CI step to fail and preventing
  documentation with spelling errors from passing the quality gate.
