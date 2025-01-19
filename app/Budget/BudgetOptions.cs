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
        [Option('t', "top-up", HelpText = "Top up the budget.")]
        public bool TopUp { get; set; }
        [Option("set-initial-amount", HelpText = "Set initial amount.")]
        public bool SetInitialAmount { get; set; }
        [Option("amount", HelpText = "Amount.")]
        public string? Amount { get; set; }
        [Option('n', "name", HelpText = "Budget name")]
        public string Name { get; set; }
        [Option('i', "id", HelpText = "Budget id")]
        public string Id { get; set; }

        [Option('p', "set-default-percent", HelpText = "Set default percent.")]
        public bool SetDefaultPercent { get; set; }
        [Option("percent", HelpText = "Default percent.")]
        public string? DefaultPercent { get; set; }
    }
}
