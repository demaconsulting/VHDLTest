## DemaConsulting.PandocTool Verification

### Verification Approach

DemaConsulting.PandocTool is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes PandocTool once per document collection to
convert Markdown source files into HTML. A passing CI step constitutes evidence that
PandocTool correctly processed all input files and produced HTML output. Downstream
WeasyprintTool PDF generation provides a secondary confirmation that the HTML is
well-formed.

FileAssert runs `.fileassert.yaml` after all document generation steps and writes a TRX
result file containing named test IDs (such as `Pandoc_BuildNotesHtml` and
`Pandoc_DesignHtml`) that confirm each HTML output was produced with the expected content.
These TRX test IDs are consumed by ReqStream to provide requirements traceability evidence
for PandocTool.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (build-docs job). Node.js
must be installed and `npm install` must have been run to provide the `mermaid-filter.cmd`
filter used by PandocTool.

### Acceptance Criteria

All PandocTool steps in the `build-docs` CI job complete with exit code 0, and the
subsequent WeasyprintTool steps successfully convert each HTML output to PDF. A passing
pipeline constitutes evidence that PandocTool correctly converted all document collections.

### Test Scenarios

- **Design document conversion**: `dotnet pandoc --defaults docs/design/definition.yaml ...`
  consolidates all design Markdown files into `docs/design/design.html`. A zero exit code
  confirms that all input files were found and the HTML was produced.
- **Multiple document collection conversions**: PandocTool is invoked for the build notes,
  design, user guide, code quality, requirements, trace matrix, review plan, and review
  report collections. All conversions completing with exit code 0 confirms the tool
  operates correctly across all document types.
- **Mermaid diagram rendering**: the `--filter mermaid-filter.cmd` option is passed in
  all invocations; successful HTML output containing rendered diagram elements confirms
  the filter integration is functional.
