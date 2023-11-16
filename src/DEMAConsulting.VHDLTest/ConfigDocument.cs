using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     Configuration document class
/// </summary>
public class ConfigDocument
{
    /// <summary>
    ///     List of VHDL files
    /// </summary>
    public List<string> Files { get; set; } = new();

    /// <summary>
    ///     List of tests
    /// </summary>
    public List<string> Tests { get; set; } = new();

    /// <summary>
    ///     Read the configuration document from file
    /// </summary>
    /// <param name="filename">Configuration file</param>
    /// <returns>Configuration document</returns>
    /// <exception cref="InvalidOperationException">Thrown on read error</exception>
    public static ConfigDocument ReadFile(string filename)
    {
        // Read the file contents
        var content = File.ReadAllText(filename);

        // Build deserializer
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        // Parse the document
        var doc = deserializer.Deserialize<ConfigDocument?>(content) ??
                  throw new InvalidOperationException($"Configuration document {filename} invalid");

        // Return the document
        return doc;
    }
}