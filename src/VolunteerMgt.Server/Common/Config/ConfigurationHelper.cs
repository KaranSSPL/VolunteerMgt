namespace VolunteerMgt.Server.Common.Config;

public static class ConfigurationHelper
{
    #region fields

    // Active Services
    public static IConfiguration ActiveConfiguration { get; set; }

    #endregion

    #region ConfigurationHelper
    /// <summary>
    /// config helper
    /// </summary>
    static ConfigurationHelper()
    {
        // Toggle if DEBUG flag set
        // Load config - DEBUG
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true)
            .Build();

        // Set the active configuration
        ActiveConfiguration = configuration;

        // add more configurations.
    }
    #endregion
}
