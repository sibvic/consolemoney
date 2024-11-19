using Alba.CsConsoleFormat;
using Sibvic.ConsoleMoney.Earning;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Sibvic.ConsoleMoney.Budget
{
    public class BudgetController(IBudgetStorage budgetStorage, ISummaryStorage summaryStorage, IBudgetPrinter budgetPrinter)
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
                if (!string.IsNullOrEmpty(options.Amount))
                {
                    if (!SetInitialAmount(options, summaryStorage, out var summaries))
                    {
                        return -1;
                    }
                }
                budgetPrinter.Print(budgets);
                return 0;
            }
            if (options.Show)
            {
                var budgets = budgetStorage.Get().ToArray();
                budgetPrinter.Print(budgets);
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
                if (!SetInitialAmount(options, summaryStorage, out summaries))
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
            if (options.TopUp)
            {
                var budgets = budgetStorage.Get().ToArray();
                if (!budgets.Any(b => b.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine("Unknown budget with id " + options.Id);
                    return -1;
                }

                List<Summary> summaries;
                summaries = summaryStorage.Get().ToList();
                var summary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase));
                if (summary == null)
                {
                    Console.WriteLine("Budget with id " + options.Id + " doesn't exists");
                    return -1;
                }
                if (!double.TryParse(options.Amount.Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
                {
                    Console.WriteLine("Failed to parse amount" + options.Amount);
                    return -1;
                }
                summaries.Remove(summary);
                summaries.Add(new Summary(options.Id, summary.Amount + amount));
                summaryStorage.Save(summaries);
                budgetPrinter.Print(budgets);
            }
            return 0;
        }

        private static bool SetInitialAmount(BudgetOptions options, ISummaryStorage summaryStorage, out List<Summary> summaries)
        {
            summaries = summaryStorage.Get().ToList();
            if (summaries.Any(s => s.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("Budget with id " + options.Id + " already have initial amount");
                return false;
            }
            if (!double.TryParse(options.Amount.Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Failed to parse amount" + options.Amount);
                return false;
            }
            summaries.Add(new Summary(options.Id, amount));
            summaryStorage.Save(summaries);
            return true;
        }
    }
}
