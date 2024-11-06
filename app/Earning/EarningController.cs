using Sibvic.ConsoleMoney.Budget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Earning
{
    public class EarningController(IEarningReader earningReader, IEarningWriter earningWriter, IIncomeReader incomeReader, 
        ISummaryReader summaryReader, ISummaryWriter summaryWriter, IBudgetReader budgetReader)
    {
        public int Start(EarningOptions options)
        {
            if (options.Add)
            {
                var incomes = incomeReader.ReadFromFile("incomes.json");
                var income = incomes.FirstOrDefault(i => i.Id.Equals(options.IncomeId, StringComparison.InvariantCultureIgnoreCase));
                if (income == null)
                {
                    Console.WriteLine("Invalid income id " + options.IncomeId);
                    return -1;
                }
                double? rate = null;
                if (options.Rate != null)
                {
                    if (!double.TryParse(options.Rate, CultureInfo.InvariantCulture, out var parsedRate))
                    {
                        Console.WriteLine("Failed to parse rate " + options.Rate);
                        return -1;
                    }
                    rate = parsedRate;
                }
                var earnings = earningReader.ReadFromFile("earnings.json").ToList();
                earnings.Add(new Earning(options.IncomeId, DateTime.Now.Date, options.Amount, rate));
                earningWriter.WriteToFile("earnings.json", earnings);

                var summaries = summaryReader.ReadFromFile("summaries.json").ToList();
                var amountWithRate = options.Amount;
                if (rate.HasValue)
                {
                    amountWithRate *= rate.Value;
                }
                foreach (var distr in income.Distribushings)
                {
                    var incomeAmount = amountWithRate * distr.Percent / 100.0;
                    if (distr.BudgetId == "")
                    {
                        AddDefaultDistributions(budgetReader, income, summaries, incomeAmount);
                        continue;
                    }
                    AddIncome(summaries, incomeAmount, distr.BudgetId);
                }

                summaryWriter.WriteToFile("summaries.json", summaries);
                return 0;
            }
            return 0;
        }

        private static void AddDefaultDistributions(IBudgetReader budgetReader, Income? income, List<Summary> summaries, double incomeAmount)
        {
            var budgets = budgetReader.ReadFromFile("budgets.json")
                .Where(b => !income.Distribushings.Any(d => d.BudgetId.Equals(b.Id, StringComparison.InvariantCultureIgnoreCase)));
            foreach (var budget in budgets)
            {
                AddIncome(summaries, incomeAmount, budget.Id);
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
