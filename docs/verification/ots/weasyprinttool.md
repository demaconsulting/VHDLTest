## DemaConsulting.WeasyprintTool Verification

### Verification Approach

DemaConsulting.WeasyprintTool is verified through CI pipeline execution. The `build-docs`
job in `.github/workflows/build.yaml` invokes WeasyprintTool once per document collection
to convert HTML files into PDF/A-3u artifacts. A passing CI step constitutes evidence
that WeasyprintTool correctly rendered each HTML input and produced a valid PDF output.
The resulting PDF files are uploaded as release artifacts.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (build-docs job). The HTML
input files produced by PandocTool must be available before the WeasyprintTool steps run.

### Acceptance Criteria

All WeasyprintTool steps in the `build-docs` CI job complete with exit code 0, and the
resulting PDF files are successfully uploaded as release artifacts. A passing pipeline
constitutes evidence that WeasyprintTool correctly converted all HTML documents to
PDF/A-3u format.

### Test Scenarios

- **Design document PDF generation**: `dotnet weasyprint --pdf-variant pdf/a-3u docs/design/design.html "docs/VHDLTest Design.pdf"`
  converts the design HTML to a PDF/A-3u file. A zero exit code and a non-empty output
  file confirm successful conversion.
- **Multiple document collection conversions**: WeasyprintTool is invoked for the build
  notes, design, user guide, code quality, requirements, trace matrix, review plan, and
  review report HTML files. All conversions completing with exit code 0 confirms the tool
  operates correctly across all document types.
- **PDF/A-3u compliance**: the `--pdf-variant pdf/a-3u` flag is applied to all invocations;
  successful output confirms the tool produces archival-grade PDF/A-3u artifacts as required.
