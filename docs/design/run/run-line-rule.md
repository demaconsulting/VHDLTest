# RunLineRule Unit Design

## Overview

`RunLineRule.cs` defines a pattern rule for classifying simulator output lines
into `RunLineType` categories.

## Responsibilities

- Match output lines against a regular expression pattern
- Return the associated `RunLineType` when a match is found
