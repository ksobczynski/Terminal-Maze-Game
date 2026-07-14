using Microsoft.Extensions.Configuration;
using PROJ.Themes;

namespace PROJ.Configuration;

public sealed class Configurator
{
    private static readonly Configurator _instance = new Configurator();

    public static Configurator Instance => _instance;

    private Configurator()
    {
    }
    public IConfigurationRoot? Configuration { get; private set; }
    
    public GameSettings Configure()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(FindConfigurationBasePath())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true).Build();
        return Configuration.GetRequiredSection("GameSettings").Get<GameSettings>() ?? 
                       throw new InvalidOperationException();
    }

    public LogSettings? ConfigureLog()
    {
        return Configuration?.GetSection("LogSettings").Get<LogSettings>();
    }

    public IDungeonThemeFactory? ConfigureTheme()
    {
        var theme = Configuration?.GetRequiredSection("ThemeSettings").GetRequiredSection("Theme").Value ??
                    throw new InvalidOperationException("ThemeSettings:Theme is missing.");
         switch (theme)
         {
             case "Sea":
                 return new SeaThemeFactory();
             case "Hospital":
                 return new HospitalThemeFactory();
             default:
                 return null;
         }
    }

    private static string FindConfigurationBasePath()
    {
        string currentDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        if (File.Exists(currentDirectoryPath))
            return Directory.GetCurrentDirectory();

        string appBasePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(appBasePath))
            return AppContext.BaseDirectory;

        string projectPath = Path.Combine(Directory.GetCurrentDirectory(), "PROJ", "appsettings.json");
        if (File.Exists(projectPath))
            return Path.GetDirectoryName(projectPath) ?? Directory.GetCurrentDirectory();

        return Directory.GetCurrentDirectory();
    }
    
}
