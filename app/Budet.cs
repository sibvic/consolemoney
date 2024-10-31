using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney
{
    [Verb("budget", HelpText = "Budget management.")]
    public class BudgetOptions
    {
        [Option('a', "add", HelpText = "Add a budget.")]
        public bool Add { get; set; }
        [Option('s', "show", HelpText = "Show list of budgets.")]
        public bool Show { get; set; }
        [Option('n', "name", HelpText = "Budget name")]
        public string Name { get; set; }
        [Option('i', "id", HelpText = "id")]
        public string Id { get; set; }
    }
    public record Budget(string Name, string Id);
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
