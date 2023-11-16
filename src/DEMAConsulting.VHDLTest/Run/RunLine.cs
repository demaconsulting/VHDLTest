namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Line of text from a run
/// </summary>
/// <param name="Type">Line Type</param>
/// <param name="Text">Line Text</param>
public record RunLine(RunLineType Type, string Text);