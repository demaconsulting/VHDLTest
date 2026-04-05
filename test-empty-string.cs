// Test if empty string can be passed as config file
using DEMAConsulting.VHDLTest.Cli;
using System;

try
{
    var ctx = Context.Create(new[] { "-c", "" });
    Console.WriteLine($"ConfigFile value: '{ctx.ConfigFile}'");
    Console.WriteLine($"ConfigFile is null: {ctx.ConfigFile == null}");
    Console.WriteLine($"ConfigFile is empty: {ctx.ConfigFile == string.Empty}");
    
    var options = Options.Parse(ctx);
    Console.WriteLine("ERROR: Should have thrown!");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
}
