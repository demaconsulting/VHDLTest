## DemaConsulting.PandocTool Integration Design

### Purpose

DemaConsulting.PandocTool wraps the Pandoc document converter and converts Markdown
documentation into HTML as an intermediate step for PDF generation. It is used in the CI
documentation pipeline to produce HTML versions of all design, requirements, code quality,
and build notes documents. PandocTool is not deployed with VHDLTest.

### Features Used

- **Markdown to HTML conversion**: processes multiple input Markdown files defined in a
  `definition.yaml` file and produces a single consolidated HTML output.
- **Pandoc defaults**: each document collection has a `definition.yaml` that lists input
  files, template, and settings; PandocTool passes these as `--defaults`.
- **Mermaid diagram support**: the `--filter mermaid-filter.cmd` option renders Mermaid
  diagrams embedded in the Markdown source.
- **Metadata injection**: `--metadata version=...` and `--metadata date=...` stamp
  version and date into the document headers.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, PandocTool is invoked once
per document collection. For example, for the design document:

```bash
dotnet pandoc \
  --defaults docs/design/definition.yaml \
  --filter node_modules/.bin/mermaid-filter.cmd \
  --metadata version="${{ inputs.version }}" \
  --metadata date="$(date +'%Y-%m-%d')" \
  --output docs/design/design.html
```

The generated HTML file is then consumed by WeasyprintTool to produce the final PDF.
