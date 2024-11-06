using Sibvic.ConsoleMoney.Budget;
using System.Globalization;

namespace Sibvic.ConsoleMoney
{
    public class IncomeController(IIncomeStorage incomeStorage, IBudgetStorage budgetReader)
    {
        public int Start(IncomeOptions options)
        {
            if (options.Add)
            {
                if (string.IsNullOrEmpty(options.Name) || string.IsNullOrEmpty(options.Id))
                {
                    Console.WriteLine("Income name and id should be specified");
                    return -1;
                }
                var incomes = incomeStorage.Get().ToList();
                incomes.Add(new Income(options.Name, options.Id, []));
                incomeStorage.Save(incomes);
                return 0;
            }
            if (options.Show)
            {
                var incomes = incomeStorage.Get().ToList();
                var budgets = budgetReader.Get();
                Console.WriteLine("List of incomes:");
                foreach (var income in incomes)
                {
                    PrintIncome(income, budgets);
                }
                return 0;
            }
            if (options.SetDistribution)
            {
                var incomes = incomeStorage.Get().ToList();
                var income = incomes.FirstOrDefault(i => i.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase));
                if (income == null)
                {
                    Console.WriteLine("Unknown income " + options.Id);
                    return -1;
                }
                incomes.Remove(income);

                var budgets = budgetReader.Get();
                var budget = budgets.FirstOrDefault(b => b.Id.Equals(options.BudgetId, StringComparison.InvariantCultureIgnoreCase));
                if (budget == null)
                {
                    Console.WriteLine("Unknown budget " + options.BudgetId);
                    return -1;
                }

                var distributions = income.Distribushings.ToList();
                var distribution = distributions.FirstOrDefault(d => d.BudgetId.Equals(options.BudgetId, StringComparison.InvariantCultureIgnoreCase));
                distributions.Remove(distribution);
                if (!double.TryParse(options.DistributionPercent, CultureInfo.InvariantCulture, out double percent))
                {
                    Console.WriteLine("Failed to parse value " + options.DistributionPercent);
                    return -1;
                }
                distributions.Add(new IncomeDistribushing(options.BudgetId, percent));
                income = new Income(income.Name, income.Id, distributions.ToArray());
                incomes.Add(income);
                incomeStorage.Save(incomes);
                PrintIncome(income, budgets);
            }
            return 0;
        }

        private static void PrintIncome(Income? income, Budget.Budget[] budgets)
        {
            Console.Write("- " + income.Name + " (" + income.Id + ") ");
            var prefix = "";
            foreach (var budget in budgets)
            {
                var distribushing = income.Distribushings
                    .FirstOrDefault(d => d.BudgetId.Equals(budget.Id, StringComparison.InvariantCultureIgnoreCase));
                if (distribushing != null)
                {
                    Console.Write(prefix + budget.Id + "=" + distribushing.Percent + "%");
                }
                else
                {
                    Console.Write(prefix + budget.Id + "=_%");
                }
                prefix = ", ";
            }
            Console.WriteLine();
        }
    }
}
