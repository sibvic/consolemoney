using CommandLine;

namespace Sibvic.ConsoleMoney.Spending
{
    [Verb("spend", HelpText = "Spendings management.")]
    public class SpendingOptions
    {
        [Option('c', "comment", HelpText = "Comment")]
        public string Comment { get; set; }
        [Option('a', "amount", Required = true, HelpText = "Amount")]
        public string Amount { get; set; }
        [Option('b', "budget", Required = true, HelpText = "Id of a budget")]
        public string BudgetId { get; set; }
    }
}
