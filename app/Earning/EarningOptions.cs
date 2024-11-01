using CommandLine;

namespace Sibvic.ConsoleMoney.Earning
{
    [Verb("earn", HelpText = "Earning management.")]
    public class EarningOptions
    {
        [Option('a', "add", HelpText = "Add an earning.")]
        public bool Add { get; set; }
        [Option("amount", Required = true, HelpText = "Amount earned")]
        public double Amount { get; set; }
        [Option("income", HelpText = "Income id")]
        public string IncomeId { get; set; }
    }
}
