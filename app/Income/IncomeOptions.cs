﻿using CommandLine;

namespace Sibvic.ConsoleMoney
{
    [Verb("income", HelpText = "Invome management.")]
    public class IncomeOptions
    {
        [Option('a', "add", HelpText = "Add an income.")]
        public bool Add { get; set; }
        [Option('s', "show", HelpText = "Show list of incomes.")]
        public bool Show { get; set; }
        [Option('d', "distr", HelpText = "Set distribution.")]
        public bool SetDistribution { get; set; }
        [Option("budget", HelpText = "Budget id.")]
        public string? BudgetId { get; set; }
        [Option("percent", HelpText = "Distribution percent.")]
        public string? DistributionPercent { get; set; }
        [Option('n', "name", HelpText = "Income name")]
        public string Name { get; set; }
        [Option('i', "id", HelpText = "Income id")]
        public string Id { get; set; }
    }
}
