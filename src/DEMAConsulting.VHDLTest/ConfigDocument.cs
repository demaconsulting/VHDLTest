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

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     Configuration document class
/// </summary>
public class ConfigDocument
{
    /// <summary>
    ///     List of VHDL files
    /// </summary>
    public string[] Files { get; set; } = [];

    /// <summary>
    ///     List of tests
    /// </summary>
    public string[] Tests { get; set; } = [];

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
