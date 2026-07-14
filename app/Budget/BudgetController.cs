using System.Globalization;

namespace Sibvic.ConsoleMoney.Budget
{
    public class BudgetController(IBudgetStorage budgetStorage, ISummaryStorage summaryStorage, IBudgetPrinter budgetPrinter, IIncomeStorage incomeStorage)
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
                try
                {
                    budgets.Add(new Budget(options.Name, options.Id, ParsePercent(options.DefaultPercent)));
                }
                catch (ParsingErrorException ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
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
            if (options.SetDefaultPercent)
            {
                var budgets = budgetStorage.Get().ToList();
                var budget = FindBudget(options.Id, budgets);
                if (budget == null)
                {
                    Console.WriteLine("Unknown budget with id " + options.Id);
                    return -1;
                }
                if (budget.IsHistoric)
                {
                    Console.WriteLine("Cannot set default percent for historic budget " + options.Id);
                    return -1;
                }
                budgets.Remove(budget);
                try
                {
                    budgets.Add(new Budget(budget.Name, budget.Id, ParsePercent(options.DefaultPercent), budget.IsHistoric));
                }
                catch (ParsingErrorException ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
                budgetStorage.Save(budgets);
                budgetPrinter.Print(budgets);

                return 0;
            }
            if (options.SetInitialAmount)
            {
                var budgets = budgetStorage.Get().ToArray();
                if (!BudgetExists(options.Id, budgets))
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
                if (!BudgetExists(options.Id, budgets))
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
            if (options.Close)
            {
                return CloseBudget(options);
            }
            return 0;
        }

        private int CloseBudget(BudgetOptions options)
        {
            if (string.IsNullOrEmpty(options.Id) || string.IsNullOrEmpty(options.MoveTo))
            {
                Console.WriteLine("Budget id and move-to budget id should be specified");
                return -1;
            }
            if (options.Id.Equals(options.MoveTo, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Cannot move remaining balance to the same budget");
                return -1;
            }

            var budgets = budgetStorage.Get().ToList();
            var budget = FindBudget(options.Id, budgets);
            if (budget == null)
            {
                Console.WriteLine("Unknown budget with id " + options.Id);
                return -1;
            }
            if (budget.IsHistoric)
            {
                Console.WriteLine("Budget with id " + options.Id + " is already historic");
                return -1;
            }

            var targetBudget = FindBudget(options.MoveTo, budgets);
            if (targetBudget == null)
            {
                Console.WriteLine("Unknown budget with id " + options.MoveTo);
                return -1;
            }
            if (targetBudget.IsHistoric)
            {
                Console.WriteLine("Cannot move remaining balance to historic budget " + options.MoveTo);
                return -1;
            }

            var summaries = summaryStorage.Get().ToList();
            var sourceSummary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase));
            var remainingAmount = sourceSummary?.Amount ?? 0;
            if (sourceSummary != null)
            {
                summaries.Remove(sourceSummary);
                summaries.Add(new Summary(options.Id, 0));
            }

            if (remainingAmount != 0)
            {
                var targetSummary = summaries.FirstOrDefault(s => s.BudgetId.Equals(options.MoveTo, StringComparison.InvariantCultureIgnoreCase));
                if (targetSummary == null)
                {
                    summaries.Add(new Summary(options.MoveTo, remainingAmount));
                }
                else
                {
                    summaries.Remove(targetSummary);
                    summaries.Add(new Summary(options.MoveTo, targetSummary.Amount + remainingAmount));
                }
            }
            summaryStorage.Save(summaries);

            budgets.Remove(budget);
            budgets.Add(new Budget(budget.Name, budget.Id, null, true));
            budgetStorage.Save(budgets);

            var incomes = incomeStorage.Get().ToList();
            var incomesUpdated = false;
            for (var i = 0; i < incomes.Count; i++)
            {
                var income = incomes[i];
                var distributions = income.Distribushings
                    .Where(d => !d.BudgetId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();
                if (distributions.Length != income.Distribushings.Length)
                {
                    incomes[i] = new Income(income.Name, income.Id, distributions);
                    incomesUpdated = true;
                }
            }
            if (incomesUpdated)
            {
                incomeStorage.Save(incomes);
            }

            if (remainingAmount != 0)
            {
                Console.WriteLine("Moved " + remainingAmount + " from " + options.Id + " to " + options.MoveTo);
            }
            budgetPrinter.Print(budgets);
            return 0;
        }

        private static bool BudgetExists(string id, IEnumerable<Budget> budgets)
        {
            var budget = FindBudget(id, budgets);
            return budget != null;
        }

        private static Budget? FindBudget(string id, IEnumerable<Budget> budgets)
        {
            return budgets.FirstOrDefault(b => b.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        private static double? ParsePercent(string? defaultPercent)
        {
            if (string.IsNullOrEmpty(defaultPercent))
            {
                return null;
            }
            if (!double.TryParse(defaultPercent.Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
            {
                throw new ParsingErrorException("Failed to parse amount" + defaultPercent);
            }
            return amount;
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
