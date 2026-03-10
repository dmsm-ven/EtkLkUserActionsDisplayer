class Program
{
    static readonly PanelsBuilder builder = new();
    static readonly LogLinesReader logLinesReader = new("user_actions.log");
    private static async Task Main(string[] args)
    {
        using var refreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(3));

        while (await refreshTimer.WaitForNextTickAsync())
        {
            var currentData = await logLinesReader.GetAllLines();
            builder.RenderPanels(currentData);
        }
    }
}

public class LogLinesReader
{
    private readonly string logFileName;

    public LogLinesReader(string logFileName)
    {
        this.logFileName = logFileName;
    }
    public Task<LogLineEntry[]> GetAllLines()
    {

    }
}
