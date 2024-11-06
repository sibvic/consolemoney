using Sibvic.ConsoleMoney.Budget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingController(ISpendingReader spendingReader, ISpendingWriter spendingWriter, IBudgetStorage budgetReader,
        ISummaryReader summaryReader, ISummaryWriter summaryWriter)
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
            var spendings = spendingReader.ReadFromFile("spendings.json").ToList();
            if (!double.TryParse(options.Amount, CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Failed to parse " + options.Amount);
                return -1;
            }
            spendings.Add(new Spending(DateTime.Now.Date, options.Comment, options.BudgetId, amount));
            spendingWriter.WriteToFile("spendings.json", spendings);

            var summaries = summaryReader.ReadFromFile("summaries.json").ToList();
            var summary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.BudgetId)) ?? new Summary(options.BudgetId, 0);
            summaries.Remove(summary);
            var newAmount = summary.Amount - amount;
            summaries.Add(new Summary(options.BudgetId, newAmount));
            summaryWriter.WriteToFile("summaries.json", summaries);

            Console.WriteLine($"{budget.Name} {summary.Amount} - {options.Amount} => {newAmount}");
            return 0;
        }
    }
}
