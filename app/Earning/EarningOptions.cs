using CommandLine;

namespace Sibvic.ConsoleMoney.Earning
{
    [Verb("earn", HelpText = "Earning management.")]
    public class EarningOptions
    {
        [Option('a', "add", HelpText = "Add an earning.")]
        public bool Add { get; set; }

        [Option('s', "show", HelpText = "Show last N earnings.")]
        public bool Show { get; set; }

        [Option('n', "number", HelpText = "Number of earnings to show.")]
        public int? Number { get; set; }

        [Option("amount", HelpText = "Amount earned.")]
        public string? Amount { get; set; }

        [Option("rate", HelpText = "Exchange rate.")]
        public string? Rate { get; set; }

        [Option("income", HelpText = "Income id")]
        public string? IncomeId { get; set; }

        [Option('c', "comment", HelpText = "Comment")]
        public string? Comment { get; set; }
    }
}
