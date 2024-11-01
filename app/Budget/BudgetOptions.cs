using CommandLine;

namespace Sibvic.ConsoleMoney.Budget
{
    [Verb("budget", HelpText = "Budget management.")]
    public class BudgetOptions
    {
        [Option('a', "add", HelpText = "Add a budget.")]
        public bool Add { get; set; }
        [Option('s', "show", HelpText = "Show list of budgets.")]
        public bool Show { get; set; }
        [Option("set-initial-amount", HelpText = "Set initial amount.")]
        public bool SetInitialAmount { get; set; }
        [Option("initial-amount", HelpText = "Initial amount.")]
        public double? InitialAmount { get; set; }
        [Option('n', "name", HelpText = "Budget name")]
        public string Name { get; set; }
        [Option('i', "id", HelpText = "Budget id")]
        public string Id { get; set; }
    }
}
