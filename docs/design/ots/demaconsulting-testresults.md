## DemaConsulting.TestResults Integration Design

### Purpose

DemaConsulting.TestResults is used by VHDLTest to write TRX test results files. It provides
the serialization logic that converts the in-memory collection of `TestResult` records into
the TRX XML format consumed by CI/CD platforms, coverage tools, and requirements traceability
tools.

### Features Used

- **TRX serialization**: the library's API is used to create a test run record and serialize
  it to a TRX file at the path specified by `--results`.

### Integration Pattern

`TestResults.SaveResults` constructs a test run object by mapping each `TestResult` record to
the library's result model, then calls the library's save method with the output file path.
The operation is performed only when the `--results` option is present in the `Context`.

```csharp
// Pseudocode — actual API reflects the library's public surface
var run = new TestResultsDocument();
foreach (var result in _results)
    run.Add(result.ToTestResultEntry());
run.Save(context.ResultsFile);
```

If the output directory does not exist or the file cannot be written, the library throws an
`IOException` which propagates to the caller.
