## DemaConsulting.ReqStream Integration Design

### Purpose

DemaConsulting.ReqStream enforces requirements traceability by verifying that every
requirement defined in the requirements YAML files is linked to at least one passing test.
It generates a requirements document, a justifications document, and a traceability matrix
as part of the CI documentation pipeline. ReqStream is not deployed with VHDLTest.

### Features Used

- **Requirements processing**: reads `requirements.yaml` and all referenced requirement
  files to produce a structured requirements document.
- **Report export**: `reqstream --report ... --justifications ...` exports the requirements
  and justifications documents to markdown files.
- **Test result reading**: reads TRX test result files (from `artifacts/**/*.trx`) to
  determine which tests passed.
- **Traceability matrix generation**: produces a matrix mapping requirements to tests.
- **Enforcement mode**: `reqstream --enforce` exits with a non-zero code if any
  requirement has no linked passing test, making unproven requirements build-breaking.
- **Lint**: `reqstream --lint --requirements requirements.yaml` validates requirement
  file syntax and cross-references.
- **Tag-based filtering**: `reqstream --filter <tags>` restricts requirements processing,
  export, and enforcement to requirements whose `tags:` list intersects the given
  comma-separated tag set, enabling selective per-tag requirements/justifications export
  (see `reqstream-usage.md`).
- **Self-validation**: `reqstream --validate --results ...` writes internal test TRX results.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, ReqStream is called after all
build and test jobs have completed and their TRX artifacts have been collected:

```bash
dotnet reqstream \
  --requirements requirements.yaml \
  --tests "artifacts/**/*.trx" \
  --report docs/requirements_doc/requirements.md \
  --justifications docs/requirements_doc/justifications.md \
  --matrix docs/requirements_report/trace_matrix.md \
  --enforce
```

In `lint.ps1` and `lint.sh`, `dotnet reqstream --lint --requirements requirements.yaml`
validates requirements file consistency at development time.
