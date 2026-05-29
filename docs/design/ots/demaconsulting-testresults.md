## DemaConsulting.TestResults Integration Design

### Purpose

DemaConsulting.TestResults is used by VHDLTest to write TRX and JUnit XML test results files. It provides
the serialization logic that converts the in-memory collection of `TestResult` records into structured XML
formats consumed by CI/CD platforms, coverage tools, and requirements traceability tools.

### Features Used

- **TRX serialization**: `TrxSerializer.Serialize(testResults)` converts a
  `DemaConsulting.TestResults.TestResults` object into a TRX XML string.
- **JUnit XML serialization**: `JUnitSerializer.Serialize(testResults)` converts the same object
  into JUnit XML format for CI/CD platforms that consume JUnit reports.

### Integration Pattern

`TestResults.SaveResults` constructs a `DemaConsulting.TestResults.TestResults` object, populates it
with `DemaConsulting.TestResults.TestResult` entries mapped from the local `TestResult` records, then
selects the serializer based on the output file extension: `JUnitSerializer.Serialize` for `.xml`,
`TrxSerializer.Serialize` for all other extensions. The serializer returns a `string`, which is then
written to disk via `File.WriteAllText`. The operation is performed only when the `--results` option
is present in the `Context`.

```csharp
// Pseudocode — actual API reflects the library's public surface
var testResults = new DemaConsulting.TestResults.TestResults { Id = RunId, Name = RunName };
foreach (var test in Tests)
    testResults.Results.Add(new DemaConsulting.TestResults.TestResult { /* mapped fields */ });

var extension = Path.GetExtension(fileName).ToLowerInvariant();
var content = extension == ".xml"
    ? JUnitSerializer.Serialize(testResults)
    : TrxSerializer.Serialize(testResults);
File.WriteAllText(fileName, content);
```

If the output directory does not exist or the file cannot be written, `File.WriteAllText` throws an
`IOException` which propagates through `SaveResults` to the caller. The OTS library serializers return
a string and do not perform any file I/O themselves.
