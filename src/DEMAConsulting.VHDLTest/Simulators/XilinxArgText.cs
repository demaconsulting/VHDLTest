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

using System.Text;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Helper for safely quoting arbitrary string values (file paths, entity names) for
///     interpolation into generated Xilinx argument files (`-file`/`-f` inputs to `xvhdl`/`xelab`).
/// </summary>
/// <remarks>
///     Used by <see cref="VivadoSimulator"/> to quote file paths and test-bench entity names
///     before interpolating them into the argument files it generates. Xilinx argument files are
///     <em>not</em> TCL scripts: per Xilinx UG900, they use standard shell-style tokenization
///     where a value containing whitespace or other special characters must be wrapped in double
///     quotes, with embedded <c>"</c> and <c>\</c> characters backslash-escaped. This is
///     deliberately distinct from <see cref="TclText"/>'s brace-quoting, which would be actively
///     wrong here: the Xilinx argument parser does not strip braces, so a brace-quoted value
///     would break file/entity lookup.
/// </remarks>
internal static class XilinxArgText
{
    /// <summary>
    ///     Quotes a string value for safe interpolation into a Xilinx argument file.
    /// </summary>
    /// <remarks>
    ///     Always wraps <paramref name="value"/> in double quotes (per the UG900-documented
    ///     argument-file convention), backslash-escaping any embedded <c>\</c> or <c>"</c>
    ///     characters. The backslash is escaped first, so the escaping backslashes themselves are
    ///     not later re-escaped.
    /// </remarks>
    /// <param name="value">The value to quote. Must not be null.</param>
    /// <returns>A double-quoted representation of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string Quote(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var builder = new StringBuilder();
        builder.Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '\\':
                case '"':
                    builder.Append('\\').Append(c);
                    break;

                default:
                    builder.Append(c);
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }
}
