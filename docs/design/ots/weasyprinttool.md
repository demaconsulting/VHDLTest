## DemaConsulting.WeasyprintTool Integration Design

### Purpose

DemaConsulting.WeasyprintTool wraps the WeasyPrint CSS-based PDF renderer and converts
HTML documents into PDF/A-3u compliance artifacts. It is used in the CI documentation
pipeline to produce formal PDF versions of all design, requirements, code quality, and
build notes documents. WeasyprintTool is not deployed with VHDLTest.

### Version Used

DemaConsulting.WeasyprintTool 68.1.0 (dotnet tool `demaconsulting.weasyprinttool`).

### Features Used

- **HTML to PDF conversion**: reads an HTML input file and writes a PDF output file.
- **PDF/A-3u variant**: the `--pdf-variant pdf/a-3u` option produces an archival-grade
  PDF/A-3u file suitable for formal review and long-term retention.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, WeasyprintTool is invoked once
per document collection after PandocTool has produced the HTML. For example, for the
design document:

```bash
dotnet weasyprint \
  --pdf-variant pdf/a-3u \
  docs/design/design.html \
  "docs/VHDLTest Design.pdf"
```

The resulting PDF files are uploaded as release artifacts and attached to GitHub releases.
