using Humanizer;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

public class PanelsBuilder
{
    private readonly List<LogLineEntry> lines = new();

    public int Limit { get; }

    public PanelsBuilder(int limit)
    {
        Limit = limit;
    }
    public void RenderPanels(IEnumerable<LogLineEntry> currentData, int maxAgeInDays)
    {
        if (currentData.Count() == 0)
            return;

        lines.AddRange(currentData);

        var generalPanel = CreateGeneralDataPanel();
        var userPanel = CreateUserPanel();

        var layout = new Columns(generalPanel, userPanel) { Expand = true };

        AnsiConsole.Clear();
        AnsiConsole.WriteLine($"История действий пользователей за последние {maxAgeInDays} дн.");
        AnsiConsole.Write(layout);
    }

    private IRenderable CreateUserPanel()
    {
        var userPanelData = lines
            .GroupBy(i => i.User)
            .OrderByDescending(i => i.Max(i => i.DateTimeUtc))
            .Select(i => $"{i.Key}. Последний доступ {i.Max(i => i.DateTimeUtc)}")
            .Take(Limit)
            .ToArray();

        var userPanel = new Panel(CreateList(userPanelData))
            .Header("Последние посещение страницы")
            .Border(BoxBorder.Rounded)
            .Expand();

        return userPanel;
    }
    private IRenderable CreateGeneralDataPanel()
    {
        var data = lines
            .OrderByDescending(i => i.DateTimeUtc)
            .Take(Limit)
            .ToArray();

        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Fuchsia)
            .Expand()
            .AddColumn(new TableColumn("[u]URL[/]").NoWrap())
            .AddColumn(new TableColumn("[u]USER[/]").Centered())
            .AddColumn(new TableColumn("[u]DATE_TIME[/]").Centered())
            .AddColumn(new TableColumn("[u]ELAPSED[/]").RightAligned());

        foreach (var row in data)
        {
            table.AddRow(
                $"[blue]{row.Url}[/]",
                $"[yellow]{row.User}[/]",
                $"[white]{row.DateTimeUtc}[/]",
                $"[white]{(DateTime.Now - row.DateTimeUtc).Humanize()}[/]"
                );
        }


        return table;
    }

    private static Markup CreateList(IEnumerable<string> items)
    {
        var markupBuilder = new StringBuilder();

        if (items.Any())
        {
            foreach (var item in items)
                markupBuilder.Append($"[green]{item}[/]\n\n");
        }
        else
        {
            markupBuilder.Append("[yellow]Нет данных[/]");
        }

        return new Markup(markupBuilder.ToString());
    }
}