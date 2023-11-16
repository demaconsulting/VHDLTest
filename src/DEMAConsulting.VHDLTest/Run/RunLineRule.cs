using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Run Line Rule Record
/// </summary>
/// <param name="Type">Run Line Type</param>
/// <param name="Pattern">Text Match Pattern</param>
public record RunLineRule(RunLineType Type, Regex Pattern);