using Alba.CsConsoleFormat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sibvic.ConsoleMoney.Earning
{
    public interface IEarningsPrinter
    {
        void PrintLastNEarnings(IEnumerable<Earning> earnings, int n);
    }

    public class ConsoleEarningsPrinter : IEarningsPrinter
    {
        public void PrintLastNEarnings(IEnumerable<Earning> earnings, int n)
        {
            var lastNEarnings = earnings.OrderByDescending(e => e.Date).Take(n).ToList();
            
            if (!lastNEarnings.Any())
            {
                Console.WriteLine("No earnings found.");
                return;
            }

            Console.WriteLine($"Last {n} earnings:");
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
                        .AddChildren("Date"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Amount"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Income ID"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Rate"),
                    new Cell { Stroke = LineThickness.Double, Color = ConsoleColor.White }
                        .AddChildren("Comment")
                );

            // Data rows
            foreach (var earning in lastNEarnings)
            {
                table.AddChildren(
                    new Cell { Stroke = LineThickness.None }
                        .AddChildren(earning.Date.ToString("yyyy-MM-dd")),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(earning.Amount.ToString("F2")),
                    new Cell { Stroke = LineThickness.None }
                        .AddChildren(earning.IncomeId),
                    new Cell { Stroke = LineThickness.None, Align = Align.Right }
                        .AddChildren(earning.Rate.HasValue ? earning.Rate.Value.ToString("F2") : "N/A"),
                    new Cell { Stroke = LineThickness.None }
                        .AddChildren(earning.Comment ?? "")
                );
            }

            ConsoleRenderer.RenderDocument(new Document().AddChildren(table));
        }
    }
} 