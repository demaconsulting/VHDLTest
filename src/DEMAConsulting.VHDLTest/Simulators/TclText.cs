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
///     interpolation into generated TCL do-scripts.
/// </summary>
/// <remarks>
///     Used by <see cref="ModelSimSimulator"/> and <see cref="ActiveHdlSimulator"/> to quote
///     file paths and test-bench entity names before interpolating them into the TCL do-scripts
///     they generate, so that values containing spaces or TCL metacharacters (<c>$</c>,
///     <c>[</c>, <c>]</c>, <c>"</c>, <c>\</c>) are treated as literal text rather than being
///     substituted or split by the TCL interpreter.
/// </remarks>
internal static class TclText
{
    /// <summary>
    ///     Quotes a string value for safe interpolation into a TCL do-script.
    /// </summary>
    /// <remarks>
    ///     Prefers TCL brace-quoting (<c>{...}</c>) because, unlike double-quoting, braces
    ///     suppress <em>all</em> TCL substitution (variable, command, and backslash) for the
    ///     enclosed text — every character between the braces is treated as a literal, so
    ///     spaces, <c>$</c>, <c>[</c>, <c>]</c>, and <c>"</c> all pass through unchanged with no
    ///     escaping required. Brace-quoting only works when <paramref name="value"/> itself
    ///     contains no literal <c>{</c>/<c>}</c> characters (braces must be balanced and
    ///     unescaped inside a braced word); when it does, this method falls back to a
    ///     double-quoted form with each of <c>\</c>, <c>"</c>, <c>$</c>, <c>[</c>, <c>]</c>,
    ///     <c>{</c>, and <c>}</c> individually backslash-escaped (backslash escaped first, so
    ///     the escaping backslashes themselves are not later re-escaped).
    /// </remarks>
    /// <param name="value">The value to quote. Must not be null.</param>
    /// <returns>A TCL-quoted representation of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string Quote(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Prefer brace-quoting: braces suppress all TCL substitution, so the value passes
        // through completely unchanged. Only valid when the value contains no literal braces.
        if (!value.Contains('{') && !value.Contains('}'))
        {
            return "{" + value + "}";
        }

        // Fall back to double-quoting with explicit backslash-escaping of every TCL
        // metacharacter that would otherwise trigger substitution inside a quoted word.
        var builder = new StringBuilder();
        builder.Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '\\':
                case '"':
                case '$':
                case '[':
                case ']':
                case '{':
                case '}':
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
