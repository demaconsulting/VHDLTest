## YamlDotNet Integration Design

### Purpose

YamlDotNet is used by VHDLTest to deserialize YAML configuration files into .NET objects.
It provides the deserialization engine that converts the on-disk YAML test configuration
into a `ConfigDocument` instance, which is then validated and promoted to an `Options` record
for use by the rest of the application.

### Version Used

YamlDotNet 18.0.0 (NuGet package `YamlDotNet`).

### Features Used

- **Deserializer**: `YamlDotNet.Serialization.Deserializer` is used to deserialize the YAML
  configuration file into a `ConfigDocument` instance.
- **Naming conventions**: property names in the YAML file use camelCase; YamlDotNet's naming
  convention support maps these to the PascalCase .NET property names.

### Integration Pattern

`ConfigDocument.ReadFile` opens the configuration file and passes the stream to a
`YamlDotNet.Serialization.Deserializer` instance. The deserializer maps YAML fields to
`ConfigDocument` properties. If the YAML is malformed or a required field is missing, a
`YamlDotNet.Core.YamlException` is thrown and propagated to the caller.

```csharp
var deserializer = new DeserializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();
var document = deserializer.Deserialize<ConfigDocument>(reader);
```

The `ConfigDocument` type mirrors the YAML schema with nullable properties so that optional
fields remain null when absent, allowing `Options.Parse` to apply defaults.
