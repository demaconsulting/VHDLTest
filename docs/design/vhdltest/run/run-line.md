# RunLine Unit Design

## Overview

`RunLine.cs` represents a single line of output captured from a simulator execution.

## Data Model

| Property   | Type          | Description                         |
| ---------- | ------------- | ----------------------------------- |
| `Text`     | `string`      | The text content of the output line |
| `Type`     | `RunLineType` | The classification of this line     |
