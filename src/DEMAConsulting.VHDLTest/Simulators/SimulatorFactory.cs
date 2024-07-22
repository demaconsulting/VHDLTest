namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Simulator Factory Class
/// </summary>
public static class SimulatorFactory
{
    /// <summary>
    ///     Array of simulators
    /// </summary>
    private static readonly Simulator[] Simulators =
    {
        GhdlSimulator.Instance,
        ModelSimSimulator.Instance,
        VivadoSimulator.Instance,
        ActiveHdlSimulator.Instance,
        NvcSimulator.Instance
    };

    /// <summary>
    ///     Get a simulator
    /// </summary>
    /// <param name="name">Simulator name</param>
    /// <returns>Simulator instance or null</returns>
    public static Simulator? Get(string? name = null)
    {
        // If the simulator is specified then use it
        if (name != null)
            return Array.Find(Simulators, s => s.SimulatorName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        // Return the first available
        return Array.Find(Simulators, s => s.Available());
    }
}