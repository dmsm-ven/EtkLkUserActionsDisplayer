using System.Text;

public class LogLinesReader
{
    private readonly string logFileName;
    private long _lastPosition = 0;
    public LogLinesReader(string logFileName)
    {
        this.logFileName = logFileName;
    }
    public async Task<IEnumerable<LogLineEntry>> GetAllLines()
    {
        if (!File.Exists(logFileName))
            throw new FileNotFoundException(logFileName);

        var result = new List<LogLineEntry>();

        using var fs = new FileStream(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        if (fs.Length < _lastPosition)
            _lastPosition = 0;

        fs.Seek(_lastPosition, SeekOrigin.Begin);

        using var reader = new StreamReader(fs, Encoding.UTF8);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (LogLineEntry.TryParse(line, out var entry))
            {
                result.Add(entry!);
            }
        }

        _lastPosition = fs.Position;

        return result;
    }
}
