class Program
{
    static string logFilePath = "user_actions.log";
    static LogLinesReader logLinesReader = new(logFilePath);
    static readonly ManualResetEvent _waitHandle = new(false);
    static readonly PanelsBuilder builder = new();

    private static async Task Main(string[] args)
    {
        if (args.Length == 2 && File.Exists(args[1]))
        {
            logFilePath = args[1];
        }

        logLinesReader = new(logFilePath);

        using var watcher = new FileSystemWatcher
        {
            Path = AppContext.BaseDirectory,   // папка запуска программы
            Filter = logFilePath,                  // только .log файлы
            NotifyFilter = NotifyFilters.LastWrite
                         | NotifyFilters.FileName
                         | NotifyFilters.Size
        };
        watcher.Changed += async (o, e) =>
        {
            _ = Render();
        };
        watcher.EnableRaisingEvents = true;

        _ = Render();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            _waitHandle.Set();
        };

        _waitHandle.WaitOne(); // приложение "ждет"
    }

    private static async Task Render()
    {
        var currentData = await logLinesReader.GetAllLines();
        builder.RenderPanels(currentData);
    }
}
