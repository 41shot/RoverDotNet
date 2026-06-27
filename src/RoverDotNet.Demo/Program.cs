using Microsoft.Extensions.DependencyInjection;

namespace RoverDotNet.Demo
{
    internal static class Program
    {
        /// <summary>
        /// Gets the service provider for the application.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Configure dependency injection
            var services = new ServiceCollection();
            services.ConfigureServices();
            ServiceProvider = services.BuildServiceProvider();

            // Create main form with DI
            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
    }
}