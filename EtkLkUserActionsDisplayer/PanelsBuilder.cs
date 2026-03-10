using Spectre.Console;
using System.Text;

public class PanelsBuilder
{
    private readonly List<LogLineEntry> lines = new();
    public void RenderPanels(LogLineEntry[] currentData)
    {
        if (currentData.Length == 0)
            return;

        lines.AddRange(currentData);

        var generalPanelData = lines.GroupBy(i => i.Url).OrderByDescending(i => i.Count()).Select(i => $"{i.Key} - {i.Count()}").ToArray();
        var generalPanel = CreateDataPanel("Общие данные", generalPanelData);

        var userPanelData = lines.GroupBy(i => i.User).OrderByDescending(i => i.Count()).Select(i => $"{i.Key} - {i.Count()}").ToArray();
        var userPanel = CreateDataPanel("Отдельно по пользователю", userPanelData);

        var layout = new Columns(generalPanel, userPanel) { Expand = true };

        AnsiConsole.Write(layout);
    }


    private static Panel CreateDataPanel(string header, string[] elements)
    {
        var userPanel = new Panel(CreateList(elements))
            .Header(header)
            .Border(BoxBorder.Rounded)
            .Expand();
        return userPanel;
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