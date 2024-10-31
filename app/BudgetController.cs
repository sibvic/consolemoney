namespace Sibvic.ConsoleMoney
{
    public class BudgetController(BudgetOptions options, IBudgetReader budgetReader, IBudgetWriter budgetWriter)
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
                var budgets = budgetReader.ReadFromFile("budgets.json").ToList();
                Console.WriteLine("List of budgets:");
                foreach (var budget in budgets)
                {
                    Console.WriteLine("- " + budget.Name + "(" + budget.Id + ")");
                }
            }
            return 0;
        }
    }
}
