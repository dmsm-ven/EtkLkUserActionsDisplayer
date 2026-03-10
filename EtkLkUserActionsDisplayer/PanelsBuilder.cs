using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

public class PanelsBuilder
{
    private readonly List<LogLineEntry> lines = new();
    public void RenderPanels(IEnumerable<LogLineEntry> currentData)
    {
        if (currentData.Count() == 0)
            return;

        lines.AddRange(currentData);

        var generalPanel = CreateGeneralDataPanel();
        var userPanel = CreateUserPanel();

        var layout = new Columns(generalPanel, userPanel) { Expand = true };

        AnsiConsole.Clear();
        AnsiConsole.Write(layout);
    }

    private IRenderable CreateUserPanel()
    {
        var userPanelData = lines.GroupBy(i => i.User)
            .OrderByDescending(i => i.Count())
            .Select(i => $"{i.Key} - {i.Count()}")
            .ToArray();

        var userPanel = new Panel(CreateList(userPanelData))
            .Header("Отдельно по пользователю")
            .Border(BoxBorder.Rounded)
            .Expand();

        return userPanel;
    }
    private IRenderable CreateGeneralDataPanel()
    {
        var data = lines
            .GroupBy(i => i.Url)
            .OrderByDescending(i => i.Count())
            .ToArray();

        int totalCount = lines.Count;

        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Fuchsia)
            .Expand()
            .AddColumn(new TableColumn("[u]URL[/]").NoWrap())
            .AddColumn(new TableColumn("[u]OPEN_COUNT[/]").Footer($"total: {totalCount}").Centered().NoWrap())
            .AddColumn(new TableColumn("[u]LAST_ACCES_DATE[/]"))
            .AddColumn(new TableColumn("[u]LAST_ACCESS_USER[/]").NoWrap());

        foreach (var row in data)
        {
            var lastAccess = row.Max(i => i.DateTimeUtc);
            var lastAccessUser = row.FirstOrDefault(i => i.DateTimeUtc == lastAccess)?.User ?? "???";
            table.AddRow(
                $"[blue]{row.Key}[/]",
                $"[yellow]{row.Count()}[/]",
                $"[white]{lastAccess}[/]",
                $"[white]{lastAccessUser}[/]"
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