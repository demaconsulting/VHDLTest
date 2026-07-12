### RunLine

![Run Structure](RunView.svg)

#### Purpose

`RunLine` is an immutable positional record pairing a single line of simulator output
text with its assigned classification type. It is the element type of the
`RunResults.Lines` collection.

#### Data Model

| Property | Type          | Description                                                   |
| -------- | ------------- | ------------------------------------------------------------- |
| `Type`   | `RunLineType` | Classification assigned to this line by `RunProcessor.Parse`. |
| `Text`   | `string`      | The text content of the output line, unmodified.              |

#### Key Methods

N/A — `RunLine` is an immutable positional record. No methods beyond the
record-generated constructor, equality members, and `Deconstruct` are defined.

#### Error Handling

N/A — immutable record with no fallible operations.

#### Dependencies

- **RunLineType** — the type of the `Type` property.

#### Callers

- Instantiated by `RunProcessor.Parse` for each line of simulator output.
- Accessed by `RunResults.Print` to determine line color for console display.
- Accessed by `RunProcessor.Parse` to compute the run summary classification.
- Accessed by `TestResults.SaveResults` (Results subsystem) to enumerate output lines when saving test results.
