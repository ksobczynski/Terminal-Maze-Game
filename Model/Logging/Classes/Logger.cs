using PROJ.Configuration;
using PROJ.GameConstansts;

namespace PROJ.Logging.Classes;

public sealed class Logger
{
    private static readonly Logger _instance = new Logger();
    private readonly object sync = new();
    private readonly List<string> logs = new();
    private int firstVisibleIndex;

    public static Logger GetInstance => _instance;

    private LogSettings? _settings { get; set; }

    public string filePath { get; set; } = string.Empty;

    private Logger()
    {
        LoggerMode = false;
        _settings = GetLogSettings();

        filePath = _settings.OutPath.Replace("{date}", DateTime.Now.ToString("yyyy-MM-dd_HH-mm"));
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filePath, $"Logs From Gameplay on day {DateTime.Now:yyyy-MM-dd_HH-mm}{Environment.NewLine}");
    }

    public void Log(string message)
    {
        lock (sync)
        {
            bool wasAtBottom = firstVisibleIndex >= MaxFirstVisibleIndex;
            logs.Add(message);

            if (wasAtBottom)
                firstVisibleIndex = MaxFirstVisibleIndex;

            File.AppendAllText(filePath, message + Environment.NewLine);
        }
    }

    public void LogUp()
    {
        lock (sync)
        {
            if (firstVisibleIndex > 0)
                firstVisibleIndex--;
        }
    }

    public void LogDown()
    {
        lock (sync)
        {
            if (firstVisibleIndex < MaxFirstVisibleIndex)
                firstVisibleIndex++;
        }
    }

    public bool LoggerMode { get; set; }

    public IReadOnlyList<string> GetVisibleLogs()
    {
        lock (sync)
        {
            return logs
                .Skip(firstVisibleIndex)
                .Take(VisibleLines)
                .ToList();
        }
    }

    private int VisibleLines => GameConstants.LoggerBoxBottom - GameConstants.LoggerBoxWritingPointStartTop;

    private int MaxFirstVisibleIndex => Math.Max(0, logs.Count - VisibleLines);

    private static LogSettings GetLogSettings()
    {
        LogSettings? settings = Configurator.Instance.ConfigureLog();
        if (settings != null)
            return settings;

        try
        {
            Configurator.Instance.Configure();
            settings = Configurator.Instance.ConfigureLog();
            if (settings != null)
                return settings;
        }
        catch
        {
        }

        return new LogSettings
        {
            OutPath = Path.Combine(AppContext.BaseDirectory, "Logs", "gameplay-{date}")
        };
    }
}
