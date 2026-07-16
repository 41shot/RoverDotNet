namespace RoverDotNet.Coprocessor;

/// <summary>
/// Entry point for running the coprocessor as a standalone ASP.NET Core application.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the standalone coprocessor host.
    /// </summary>
    public static void Main(string[] args)
    {
        var app = CoprocessorAppBuilder.Build(args);
        app.Run();
    }
}
