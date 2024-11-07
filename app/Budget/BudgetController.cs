using Alba.CsConsoleFormat;
using System.ComponentModel;
using System.Diagnostics;

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

                if (options.InitialAmount.HasValue && !SetInitialAmount(options, summaryStorage, out var summaries))
                {
                    return -1;
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
            summaries.Add(new Summary(options.Id, options.InitialAmount.Value));
            summaryStorage.Save(summaries);
            return true;
        }
    }
}
