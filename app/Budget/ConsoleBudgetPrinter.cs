using Alba.CsConsoleFormat;

namespace Sibvic.ConsoleMoney.Budget
{
    public class ConsoleBudgetPrinter(ISummaryStorage summaryStorage) : IBudgetPrinter
    {
        public void Print(IEnumerable<Budget> budgets)
        {
            var summaries = summaryStorage.Get();
            Console.WriteLine("List of budgets:");
            var table = new Grid { Stroke = LineThickness.Double, StrokeColor = ConsoleColor.DarkGray }
                .AddColumns(
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto }
                )
                .AddChildren(
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Name"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Id"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Amount"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Def %"),
                    budgets.Select(budget => new[] {
                        new Cell { Stroke = LineThickness.None }
                            .AddChildren(budget.Name),
                        new Cell { Stroke = LineThickness.None }
                            .AddChildren(budget.Id),
                        new Cell { Stroke = LineThickness.None, Align = Align.Right }
                            .AddChildren(summaries.Where(s => s.BudgetId.Equals(budget.Id, StringComparison.InvariantCultureIgnoreCase))
                            .Select(s => s.Amount)
                            .FirstOrDefault(0).ToString("n0")),
                        new Cell { Stroke = LineThickness.None, Align = Align.Right }
                            .AddChildren(budget.DefaultPercent == null ? "" : budget.DefaultPercent.Value.ToString("0.00"))
                    })
                );
            ConsoleRenderer.RenderDocument(new Document().AddChildren(table));
        }
    }
}
