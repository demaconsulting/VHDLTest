## DemaConsulting.WeasyprintTool Verification

### Verification Approach

DemaConsulting.WeasyprintTool is verified through CI pipeline execution. The `build-docs`
job in `.github/workflows/build.yaml` invokes WeasyprintTool once per document collection
to convert HTML files into PDF/A-3u artifacts. A passing CI step constitutes evidence
that WeasyprintTool correctly rendered each HTML input and produced a valid PDF output.
The resulting PDF files are uploaded as release artifacts.

FileAssert runs `.fileassert.yaml` after all document generation steps and writes a TRX
result file containing named test IDs (such as `WeasyPrint_BuildNotesPdf` and
`WeasyPrint_DesignPdf`) that confirm each PDF output was produced with the expected
metadata and content. These TRX test IDs are consumed by ReqStream to provide
requirements traceability evidence for WeasyprintTool.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (build-docs job). The HTML
input files produced by PandocTool must be available before the WeasyprintTool steps run.

### Acceptance Criteria

All WeasyprintTool steps in the `build-docs` CI job complete with exit code 0, and the
resulting PDF files are successfully uploaded as release artifacts. A passing pipeline
constitutes evidence that WeasyprintTool correctly converted all HTML documents to
PDF/A-3u format.

### Test Scenarios

- **WeasyPrint_BuildNotesPdf**: FileAssert runs `.fileassert.yaml` with the `build-notes`
  tag and confirms that `docs/VHDLTest Build Notes.pdf` was produced by WeasyprintTool:
  the file exists, carries the expected PDF metadata (Title, Author, Subject), has at least
  one page, and includes the text "Build Notes". A passing FileAssert check confirms
  WeasyprintTool correctly converted the build notes HTML to PDF/A-3u format.
- **WeasyPrint_DesignPdf**: FileAssert runs `.fileassert.yaml` with the `design` tag and
  confirms that `docs/VHDLTest Design.pdf` was produced by WeasyprintTool: the file exists,
  carries the expected PDF metadata (Title, Author, Subject), has at least three pages, and
  includes the text "Design". A passing FileAssert check confirms WeasyprintTool correctly
  converted the design HTML to PDF/A-3u format.
- **Design document PDF generation**: `dotnet weasyprint --pdf-variant pdf/a-3u docs/design/design.html "docs/VHDLTest Design.pdf"`
  converts the design HTML to a PDF/A-3u file. A zero exit code and a non-empty output
  file confirm successful conversion.
- **Multiple document collection conversions**: WeasyprintTool is invoked for the build
  notes, design, user guide, code quality, requirements, trace matrix, review plan, and
  review report HTML files. All conversions completing with exit code 0 confirms the tool
  operates correctly across all document types.
- **PDF/A-3u compliance**: the `--pdf-variant pdf/a-3u` flag is applied to all invocations;
  successful output confirms the tool produces archival-grade PDF/A-3u artifacts as required.
