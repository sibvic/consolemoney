namespace Sibvic.ConsoleMoney
{
    public class BudgetController(BudgetOptions options, IBudgetReader budgetReader, IBudgetWriter budgetWriter, ISummaryReader summaryReader, ISummaryWriter summaryWriter)
    {
        public int Start()
        {
            if (options.Add)
            {
                if (string.IsNullOrEmpty(options.Name) || string.IsNullOrEmpty(options.Id))
                {
                    Console.WriteLine("Budget name and id should be specified");
                    return -1;
                }
                var budgets = budgetReader.ReadFromFile("budgets.json").ToList();
                budgets.Add(new Budget(options.Name, options.Id));
                budgetWriter.WriteToFile("budgets.json", budgets);
                return 0;
            }
            if (options.Show)
            {
                var budgets = budgetReader.ReadFromFile("budgets.json").ToArray();
                Console.WriteLine("List of budgets:");
                foreach (var budget in budgets)
                {
                    Console.WriteLine("- " + budget.Name + "(" + budget.Id + ")");
                }
                return 0;
            }
            if (options.SetInitialAmount)
            {
                var budgets = budgetReader.ReadFromFile("budgets.json").ToArray();
                if (!budgets.Any(b => b.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine("Unknown budget with id " + options.Id);
                    return -1;
                }
                var summaries = summaryReader.ReadFromFile("summaries.json").ToList();
                if (summaries.Any(s => s.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine("Budget with id " + options.Id + " already have initial amount");
                    return -1;
                }
                summaries.Add(new Summary(options.Id, options.InitialAmount));
                summaryWriter.WriteToFile("summaries.json", summaries);
                Console.WriteLine("Summary:");
                foreach (var summary in summaries)
                {
                    Console.WriteLine("- " + summary.BudgetId + ": " + summary.Amount);
                }
                return 0;
            }
            return 0;
        }
    }
}
