using Alba.CsConsoleFormat;
using System.ComponentModel;
using System.Diagnostics;

namespace Sibvic.ConsoleMoney.Budget
{
    public class BudgetController(IBudgetStorage budgetStorage, ISummaryReader summaryReader, ISummaryWriter summaryWriter)
    {
        public int Start(BudgetOptions options)
        {
            if (options.Add)
            {
                if (string.IsNullOrEmpty(options.Name) || string.IsNullOrEmpty(options.Id))
                {
                    Console.WriteLine("Budget name and id should be specified");
                    return -1;
                }
                var budgets = budgetStorage.Get().ToList();
                budgets.Add(new Budget(options.Name, options.Id));
                budgetStorage.Save(budgets);

                if (options.InitialAmount.HasValue && !SetInitialAmount(options, summaryReader, summaryWriter, out var summaries))
                {
                    return -1;
                }
                ShowList(budgets);
                return 0;
            }
            if (options.Show)
            {
                var budgets = budgetStorage.Get().ToArray();
                ShowList(budgets);
                return 0;
            }
            if (options.SetInitialAmount)
            {
                var budgets = budgetStorage.Get().ToArray();
                if (!budgets.Any(b => b.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine("Unknown budget with id " + options.Id);
                    return -1;
                }
                List<Summary> summaries;
                if (!SetInitialAmount(options, summaryReader, summaryWriter, out summaries))
                {
                    return -1;
                }
                Console.WriteLine("Summary:");
                foreach (var summary in summaries)
                {
                    Console.WriteLine("- " + summary.BudgetId + ": " + summary.Amount);
                }
                return 0;
            }
            return 0;
        }

        private void ShowList(IEnumerable<Budget> budgets)
        {
            var summaries = summaryReader.ReadFromFile("summaries.json");
            Console.WriteLine("List of budgets:");
            var table = new Grid { Stroke = LineThickness.Double, StrokeColor = ConsoleColor.DarkGray }
                .AddColumns(
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
                    budgets.Select(budget => new[] {
                        new Cell { Stroke = LineThickness.None }
                            .AddChildren(budget.Name),
                        new Cell { Stroke = LineThickness.None }
                            .AddChildren(budget.Id),
                        new Cell { Stroke = LineThickness.None, Align = Align.Right }
                            .AddChildren(summaries.Where(s => s.BudgetId.Equals(budget.Id, StringComparison.InvariantCultureIgnoreCase))
                            .Select(s => s.Amount)
                            .FirstOrDefault(0).ToString("n0")),
                    })
                );
            ConsoleRenderer.RenderDocument(new Document().AddChildren(table));
        }

        private static bool SetInitialAmount(BudgetOptions options, ISummaryReader summaryReader, ISummaryWriter summaryWriter, out List<Summary> summaries)
        {
            summaries = summaryReader.ReadFromFile("summaries.json").ToList();
            if (summaries.Any(s => s.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("Budget with id " + options.Id + " already have initial amount");
                return false;
            }
            summaries.Add(new Summary(options.Id, options.InitialAmount.Value));
            summaryWriter.WriteToFile("summaries.json", summaries);
            return true;
        }
    }
}
