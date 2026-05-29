// Copyright (c) 2023 DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DEMAConsulting.VHDLTest.Cli;

/// <summary>
///     Represents the deserialized content of a VHDLTest YAML configuration file, providing lists of VHDL source files and test names.
/// </summary>
/// <remarks>
///     ConfigDocument is a stateless deserialization target for YamlDotNet. Instances are
///     effectively immutable value objects after <see cref="ReadFile"/> returns; callers
///     should treat the <see cref="Files"/> and <see cref="Tests"/> arrays as read-only.
///     The YAML property names use hyphenated-naming-convention (e.g., <c>files</c>,
///     <c>tests</c>) mapped by <c>HyphenatedNamingConvention</c>.
/// </remarks>
public class ConfigDocument
{
    /// <summary>
    ///     Gets or sets the list of VHDL source files to compile.
    /// </summary>
    /// <value>
    ///     An array of relative or absolute paths to VHDL source files, populated from the
    ///     <c>files</c> YAML key (using the <c>HyphenatedNamingConvention</c>). Defaults to
    ///     an empty array when the key is absent from the YAML document. Callers may iterate
    ///     this collection safely without a null check.
    /// </value>
    public string[] Files { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of VHDL test bench entity names to run.
    /// </summary>
    /// <value>
    ///     An array of VHDL test bench entity names, populated from the <c>tests</c> YAML
    ///     key (using the <c>HyphenatedNamingConvention</c>). Defaults to an empty array when
    ///     the key is absent from the YAML document. Callers may iterate this collection
    ///     safely without a null check.
    /// </value>
    public string[] Tests { get; set; } = [];

    /// <summary>
    ///     Reads and deserializes a VHDLTest YAML configuration file into a <see cref="ConfigDocument"/> instance.
    /// </summary>
    /// <remarks>
    ///     Encapsulates deserialization so callers receive a stable exception contract
    ///     (<see cref="FileNotFoundException"/> or <see cref="InvalidOperationException"/>) regardless
    ///     of the underlying YAML library behavior. <see cref="HyphenatedNamingConvention"/> is used
    ///     so YAML keys match natural hyphenated style (e.g., a multi-word property such as
    ///     <c>SomeKey</c> would map from <c>some-key</c> in the YAML) while C# properties use
    ///     PascalCase. All exceptions raised during deserialization are caught and wrapped as
    ///     <see cref="InvalidOperationException"/> to prevent library-internal types from leaking into
    ///     callers. The method is stateless and thread-safe; multiple threads may call it concurrently
    ///     with independent file paths without synchronization.
    /// </remarks>
    /// <param name="filename">
    ///     Path to a readable YAML file containing valid VHDLTest configuration. Must point to
    ///     an existing file whose content can be deserialized into a <see cref="ConfigDocument"/>.
    /// </param>
    /// <returns>
    ///     A non-null <see cref="ConfigDocument"/> instance populated from the file content. The
    ///     returned instance always has non-null <see cref="Files"/> and <see cref="Tests"/> arrays
    ///     (defaulting to empty arrays when the properties are absent from the YAML).
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown when the configuration file does not exist.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the configuration content is null, invalid, or cannot be deserialized into
    ///     a configuration document.
    /// </exception>
    public static ConfigDocument ReadFile(string filename)
    {
        // Read the file contents
        var content = File.ReadAllText(filename);

        // Build deserializer
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        // Parse the document
        ConfigDocument? doc;
        try
        {
            doc = deserializer.Deserialize<ConfigDocument?>(content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Configuration document {filename} is invalid", ex);
        }

        if (doc == null)
        {
            throw new InvalidOperationException($"Configuration document {filename} is null");
        }

        // Return the document
        return doc;
    }
}
