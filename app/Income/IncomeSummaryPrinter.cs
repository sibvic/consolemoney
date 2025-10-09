using Alba.CsConsoleFormat;
using Sibvic.ConsoleMoney.Earning;

namespace Sibvic.ConsoleMoney
{
    public interface IIncomeSummaryPrinter
    {
        void PrintMonthlySummary(IEnumerable<Earning.Earning> earnings, IEnumerable<Income> incomes, int months);
    }

    public class ConsoleIncomeSummaryPrinter : IIncomeSummaryPrinter
    {
        public void PrintMonthlySummary(IEnumerable<Earning.Earning> earnings, IEnumerable<Income> incomes, int months)
        {
            var now = DateTime.Now;
            var startDate = now.AddMonths(-months + 1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);

            // Filter earnings for last N months
            var recentEarnings = earnings
                .Where(e => e.Date >= startDate)
                .ToList();

            if (!recentEarnings.Any())
            {
                Console.WriteLine($"No earnings found in the last {months} months.");
                return;
            }

            // Group by year-month and income ID
            var monthlyData = recentEarnings
                .GroupBy(e => new { 
                    Year = e.Date.Year, 
                    Month = e.Date.Month, 
                    IncomeId = e.IncomeId 
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.IncomeId,
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            // Get unique income IDs from the data
            var incomeIds = monthlyData.Select(x => x.IncomeId).Distinct().OrderBy(x => x).ToList();
            
            // Create all month-year combinations for last N months
            var allMonths = Enumerable.Range(0, months)
                .Select(i => startDate.AddMonths(i))
                .Select(d => new { Year = d.Year, Month = d.Month })
                .ToList();

            Console.WriteLine($"Monthly income summary for last {months} months:");
            
            // Build column array dynamically
            var columns = new List<Column>();
            columns.Add(new Column { Width = GridLength.Auto }); // Month column
            foreach (var incomeId in incomeIds)
            {
                columns.Add(new Column { Width = GridLength.Auto });
            }
            columns.Add(new Column { Width = GridLength.Auto }); // Total column

            // Create table with dynamic columns
            var table = new Grid { Stroke = LineThickness.Double, StrokeColor = ConsoleColor.DarkGray }
                .AddColumns(columns.ToArray());

            // Header row
            table.AddChildren(new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                .AddChildren("Month"));
            
            foreach (var incomeId in incomeIds)
            {
                var income = incomes.FirstOrDefault(i => i.Id == incomeId);
                var displayName = income?.Name ?? incomeId;
                table.AddChildren(new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                    .AddChildren(displayName));
            }
            
            table.AddChildren(new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                .AddChildren("Total"));

            // Data rows
            foreach (var month in allMonths)
            {
                // Month column
                var monthStr = new DateTime(month.Year, month.Month, 1).ToString("yyyy-MM");
                table.AddChildren(new Cell { Stroke = LineThickness.None }
                    .AddChildren(monthStr));

                double monthTotal = 0;

                // Income columns
                foreach (var incomeId in incomeIds)
                {
                    var data = monthlyData.FirstOrDefault(x => 
                        x.Year == month.Year && 
                        x.Month == month.Month && 
                        x.IncomeId == incomeId);
                    
                    var amount = data?.Total ?? 0;
                    monthTotal += amount;

                    table.AddChildren(new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(amount > 0 ? amount.ToString("F2") : "-"));
                }

                // Total column
                table.AddChildren(new Cell { Stroke = LineThickness.None, Align = Align.Right, Color = ConsoleColor.Yellow }
                    .AddChildren(monthTotal.ToString("F2")));
            }

            // Calculate totals row
            table.AddChildren(new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                .AddChildren("Total"));
            
            double grandTotal = 0;
            foreach (var incomeId in incomeIds)
            {
                var incomeTotal = monthlyData
                    .Where(x => x.IncomeId == incomeId)
                    .Sum(x => x.Total);
                grandTotal += incomeTotal;

                table.AddChildren(new Cell { Stroke = LineThickness.Double, Align = Align.Right, Color = ConsoleColor.White }
                    .AddChildren(incomeTotal.ToString("F2")));
            }

            table.AddChildren(new Cell { Stroke = LineThickness.Double, Align = Align.Right, Color = ConsoleColor.Yellow }
                .AddChildren(grandTotal.ToString("F2")));

            ConsoleRenderer.RenderDocument(new Document().AddChildren(table));
        }
    }
}
