using Sibvic.ConsoleMoney.Budget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingController(ISpendingStorage spendingStorage, IBudgetStorage budgetReader,
        ISummaryStorage summaryStorage, ISpendingPrinter spendingPrinter)
    {
        public int Start(SpendingOptions options)
        {
            var budgets = budgetReader.Get();
            var budget = budgets.FirstOrDefault(b => b.Id.Equals(options.BudgetId, StringComparison.InvariantCultureIgnoreCase));
            if (budget == null)
            {
                Console.WriteLine("Unknown budget " + options.BudgetId);
                return -1;
            }
            var spendings = spendingStorage.Get().ToList();
            if (!double.TryParse(options.Amount.Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Failed to parse " + options.Amount);
                return -1;
            }
            spendings.Add(new Spending(DateTime.Now.Date, options.Comment, options.BudgetId, amount));
            spendingStorage.Save(spendings);

            var summaries = summaryStorage.Get().ToList();
            var summary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.BudgetId)) ?? new Summary(options.BudgetId, 0);
            summaries.Remove(summary);
            var newAmount = summary.Amount - amount;
            summaries.Add(new Summary(options.BudgetId, newAmount));
            summaryStorage.Save(summaries);

            spendingPrinter.PrintSpendingResult(budget, summary.Amount, amount, newAmount);
            return 0;
        }
    }
}
