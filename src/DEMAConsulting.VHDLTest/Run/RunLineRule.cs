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

using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Immutable record that pairs a compiled regular expression with the
///     <see cref="RunLineType"/> to assign when the pattern matches a simulator output line.
///     It is the rule element that <see cref="RunProcessor"/> applies in order to classify
///     each captured line; the first matching rule in the ordered rule set wins.
/// </summary>
/// <remarks>
///     Design intent: rules are evaluated in order by <see cref="RunProcessor.Parse"/> and
///     the first matching rule wins, allowing callers to express priority-ordered pattern sets.
///     A 100 ms evaluation timeout is applied to each <see cref="Regex"/> at construction
///     time as a guard against catastrophic backtracking (ReDoS) on unexpectedly long or
///     pathological simulator output lines. This type is immutable and thread-safe;
///     <see cref="Regex"/> is safe for concurrent reads, so instances may be shared freely
///     across threads without synchronization.
/// </remarks>
/// <param name="Type">
///     The severity category to assign to any output line whose text matches
///     <paramref name="Pattern"/>. Any valid <see cref="RunLineType"/> value is accepted.
/// </param>
/// <param name="Pattern">
///     The compiled regular expression used to test each simulator output line.
///     Must not be null.
/// </param>
public record RunLineRule(RunLineType Type, Regex Pattern)
{
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    /// <summary>
    ///     Gets the compiled regular expression used to test each simulator output line.
    /// </summary>
    /// <remarks>
    ///     Overrides the auto-generated positional property to enforce a null guard at
    ///     construction time. Any attempt to construct a <see cref="RunLineRule"/> directly
    ///     with a null <see cref="System.Text.RegularExpressions.Regex"/> (e.g.,
    ///     <c>new RunLineRule(type, null!)</c>) will throw <see cref="ArgumentNullException"/>
    ///     before the instance is returned to the caller. Prefer the <see cref="Create"/> factory
    ///     method, which compiles the pattern string and applies the construction-time guard.
    /// </remarks>
    public Regex Pattern { get; init; } = Pattern ?? throw new ArgumentNullException(nameof(Pattern));
    /// <summary>
    ///     Creates a new <see cref="RunLineRule"/> by compiling <paramref name="pattern"/>
    ///     into a <see cref="Regex"/> and pairing it with <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    ///     The regex is compiled with <see cref="RegexOptions.None"/> to preserve case
    ///     sensitivity, which is required for correct matching of simulator output where
    ///     keyword casing is significant. A 100 ms evaluation timeout is applied as a guard
    ///     against catastrophic backtracking (ReDoS) on unexpectedly long or pathological
    ///     simulator output lines.
    /// </remarks>
    /// <param name="type">
    ///     The <see cref="RunLineType"/> to assign to any line whose text matches
    ///     <paramref name="pattern"/>.
    /// </param>
    /// <param name="pattern">
    ///     A syntactically valid regular expression string. Must not be null.
    /// </param>
    /// <returns>A new <see cref="RunLineRule"/> whose <see cref="Pattern"/> is ready for use.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown (as <see cref="System.Text.RegularExpressions.RegexParseException"/>, a
    ///     subtype) when <paramref name="pattern"/> is not a syntactically valid regular expression.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pattern"/> is null.</exception>
    public static RunLineRule Create(RunLineType type, string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        return new RunLineRule(
            type,
            new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100)));
    }
}
