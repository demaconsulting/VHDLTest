## YamlDotNet Integration Design

### Purpose

YamlDotNet is used by VHDLTest to deserialize YAML configuration files into .NET objects.
It provides the deserialization engine that converts the on-disk YAML test configuration
into a `ConfigDocument` instance, which is then validated and promoted to an `Options` record
for use by the rest of the application.

### Features Used

- **Deserializer**: `YamlDotNet.Serialization.Deserializer` is used to deserialize the YAML
  configuration file into a `ConfigDocument` instance.
- **Naming conventions**: property names in the YAML file use hyphenated-case; YamlDotNet's naming
  convention support maps these to the PascalCase .NET property names.

### Integration Pattern

`ConfigDocument.ReadFile` reads the configuration file into a string via `File.ReadAllText` and
passes that text content to a `YamlDotNet.Serialization.Deserializer` instance. The deserializer
maps YAML fields to `ConfigDocument` properties. If the YAML is malformed or cannot be deserialized,
any exception is caught and re-thrown as an `InvalidOperationException`, ensuring a consistent
exception type for callers.

```csharp
var deserializer = new DeserializerBuilder()
    .WithNamingConvention(HyphenatedNamingConvention.Instance)
    .Build();
ConfigDocument? doc;
try
{
    doc = deserializer.Deserialize<ConfigDocument?>(content);
}
catch (Exception ex)
{
    throw new InvalidOperationException($"Configuration document {filename} is invalid", ex);
}
```

The `ConfigDocument` type mirrors the YAML schema with default-valued array properties so that
optional fields remain empty (not null) when absent, allowing `Options.Parse` to apply defaults.
