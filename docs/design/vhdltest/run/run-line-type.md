### RunLineType

#### Purpose

`RunLineType` is an enumeration classifying simulator output lines and the overall
summary of a run. Its ordinal values encode severity order, which `RunProcessor.Parse`
exploits with the `>` operator to identify the highest-severity type across all output
lines.

#### Data Model

| Value     | Ordinal | Description                                                                             |
| --------- | ------- | --------------------------------------------------------------------------------------- |
| `Text`    | 0       | Standard unclassified output. The default when no `RunLineRule` matches.                |
| `Info`    | 1       | Informational message produced by the simulator.                                        |
| `Warning` | 2       | Non-fatal warning; does not by itself cause the run summary to be classified as failed. |
| `Error`   | 3       | Error or failure condition. Causes the run summary to be `Error`.                       |

#### Key Methods

N/A — `RunLineType` is an enum. Severity ordering is provided by the language through
ordinal comparison using the `>` operator.

#### Error Handling

N/A — enum type with no methods.

#### Dependencies

None.

#### Callers

- **RunLineRule** — stores the `RunLineType` to assign when its pattern matches.
- **RunLine** — holds the `RunLineType` for each classified output line.
- **RunResults** — holds the `Summary` `RunLineType` for the overall run.
- **RunProcessor** — compares `RunLineType` values to compute the run summary and
  applies the non-zero exit code threshold.
- Simulator implementations read `RunResults.Summary` against `RunLineType.Error`
  to determine pass/fail status.
