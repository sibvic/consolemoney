using Sibvic.ConsoleMoney.Budget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Earning
{
    public class EarningController(IEarningStorage earningStorage, IIncomeStorage incomeStorage, 
        ISummaryStorage summaryStorage, IBudgetStorage budgetReader, IBudgetPrinter budgetPrinter,
        IEarningsPrinter earningsPrinter)
    {
        public int Start(EarningOptions options)
        {
            if (options.Add)
            {
                var incomes = incomeStorage.Get();
                var income = incomes.FirstOrDefault(i => i.Id.Equals(options.IncomeId, StringComparison.InvariantCultureIgnoreCase));
                if (income == null)
                {
                    Console.WriteLine("Invalid income id " + options.IncomeId);
                    return -1;
                }
                double? rate = null;
                if (options.Rate != null)
                {
                    if (!double.TryParse(options.Rate.Replace(',', '.'), CultureInfo.InvariantCulture, out var parsedRate))
                    {
                        Console.WriteLine("Failed to parse rate " + options.Rate);
                        return -1;
                    }
                    rate = parsedRate;
                }
                var earnings = earningStorage.Get().ToList();
                if (!double.TryParse(options.Amount.Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
                {
                    Console.WriteLine("Failed to parse amount " + options.Amount);
                    return -1;
                }
                earnings.Add(new Earning(options.IncomeId, DateTime.Now.Date, amount, rate, options.Comment));
                earningStorage.Save(earnings);

                var summaries = summaryStorage.Get().ToList();
                var amountWithRate = amount;
                if (rate.HasValue)
                {
                    amountWithRate *= rate.Value;
                }
                bool defaultDistributionsAdded = false;
                foreach (var distr in income.Distribushings)
                {
                    var incomeAmount = amountWithRate * distr.Percent / 100.0;
                    if (distr.BudgetId == "")
                    {
                        AddDefaultDistributions(budgetReader, income.Distribushings, summaries, incomeAmount, amountWithRate);
                        defaultDistributionsAdded = true;
                        continue;
                    }
                    AddIncome(summaries, incomeAmount, distr.BudgetId);
                }
                if (!defaultDistributionsAdded)
                {
                    AddDefaultDistributions(budgetReader, [], summaries, 0, amountWithRate);
                }

                summaryStorage.Save(summaries);
                budgetPrinter.Print(budgetReader.Get());
                return 0;
            }
            if (options.Show)
            {
                var earnings = earningStorage.Get();
                var n = options.Number ?? 10; // Default to showing last 10 earnings if not specified
                earningsPrinter.PrintLastNEarnings(earnings, n);
                return 0;
            }
            return 0;
        }

        private static void AddDefaultDistributions(IBudgetStorage budgetReader, IncomeDistribushing[] distribushings, List<Summary> summaries, double incomeAmount, 
            double amountWithRate)
        {
            var budgets = budgetReader.Get()
                .Where(b => !distribushings.Any(d => d.BudgetId.Equals(b.Id, StringComparison.InvariantCultureIgnoreCase)));
            foreach (var budget in budgets)
            {
                double amountToAdd = incomeAmount;
                if (budget.DefaultPercent != null)
                {
                    amountToAdd = amountWithRate * budget.DefaultPercent.Value / 100.0;
                }
                if (amountToAdd > 0)
                {
                    AddIncome(summaries, amountToAdd, budget.Id);
                }
            }
        }

        private static void AddIncome(List<Summary> summaries, double incomeAmount, string budgetId)
        {
            var summary = summaries.FirstOrDefault(s => s.BudgetId.Equals(budgetId, StringComparison.InvariantCultureIgnoreCase));
            if (summary == null)
            {
                summaries.Add(new Summary(budgetId, incomeAmount));
            }
            else
            {
                summaries.Remove(summary);
                summaries.Add(summary with { Amount = summary.Amount + incomeAmount });
            }
        }
    }
}
