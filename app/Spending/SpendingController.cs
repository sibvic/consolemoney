using Sibvic.ConsoleMoney.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingController(SpendingOptions options, ISpendingReader spendingReader, ISpendingWriter spendingWriter, IBudgetReader budgetReader,
        ISummaryReader summaryReader, ISummaryWriter summaryWriter)
    {
        public int Start()
        {
            var budgets = budgetReader.ReadFromFile("budgets.json");
            if (!budgets.Any(b => b.Id.Equals(options.BudgetId, StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("Unknown budget " + options.BudgetId);
                return -1;
            }
            var spendings = spendingReader.ReadFromFile("spendings.json").ToList();
            spendings.Add(new Spending(DateTime.Now.Date, options.Comment, options.BudgetId, options.Amount));
            spendingWriter.WriteToFile("spendings.json", spendings);

            var summaries = summaryReader.ReadFromFile("summaries.json").ToList();
            var summary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.BudgetId)) ?? new Summary(options.BudgetId, 0);
            summaries.Remove(summary);
            summaries.Add(new Summary(options.BudgetId, summary.Amount - options.Amount));
            summaryWriter.WriteToFile("summaries.json", summaries);
            return 0;
        }
    }
}
