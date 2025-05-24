using Alba.CsConsoleFormat;

namespace Sibvic.ConsoleMoney.Spending
{
    public interface ISpendingPrinter
    {
        void PrintSpendingResult(Budget.Budget budget, double oldAmount, double spentAmount, double newAmount);
    }

    public class ConsoleSpendingPrinter : ISpendingPrinter
    {
        public void PrintSpendingResult(Budget.Budget budget, double oldAmount, double spentAmount, double newAmount)
        {
            var table = new Grid { Stroke = LineThickness.Double, StrokeColor = ConsoleColor.DarkGray }
                .AddColumns(
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Auto }
                )
                .AddChildren(
                    // Header row
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Budget"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Previous"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Spent"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("New"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Change")
                )
                .AddChildren(
                    // Data row
                    new Cell { Stroke = LineThickness.None }
                        .AddChildren(budget.Name),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(oldAmount.ToString("F2")),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(spentAmount.ToString("F2")),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(newAmount.ToString("F2")),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right, Color = ConsoleColor.Red }
                        .AddChildren($"-{spentAmount:F2}")
                );

            ConsoleRenderer.RenderDocument(new Document().AddChildren(table));
        }
    }
} 