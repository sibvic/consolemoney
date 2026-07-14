using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Project
{
    public class ProjectController(IProjectStorage projectStorage, IProjectSummaryStorage projectSummaryStorage, IProjectPrinter projectPrinter)
    {
        public int Start(ProjectOptions options)
        {
            if (options.Add)
            {
                if (string.IsNullOrEmpty(options.Id))
                {
                    Console.WriteLine("Project id should be specified");
                    return -1;
                }
                var projects = projectStorage.Get().ToList();
                if (projects.Any(p => p.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine("Project with id " + options.Id + " already exists");
                    return -1;
                }
                projects.Add(new Project(options.Id));
                projectStorage.Save(projects);
                projectPrinter.Print(projects.ToArray(), []);
                return 0;
            }
            if (options.Show)
            {
                var projects = projectStorage.Get().ToArray();
                projectPrinter.Print(projects, []);
                return 0;
            }
            if (options.Spend)
            {
                if (string.IsNullOrEmpty(options.Id) || string.IsNullOrEmpty(options.Amount))
                {
                    Console.WriteLine("Project id and amount should be specified for spending");
                    return -1;
                }
                return ProcessTransaction(options, false);
            }
            if (options.Earn)
            {
                if (string.IsNullOrEmpty(options.Id) || string.IsNullOrEmpty(options.Amount))
                {
                    Console.WriteLine("Project id and amount should be specified for earning");
                    return -1;
                }
                return ProcessTransaction(options, true);
            }
            return 0;
        }

        private int ProcessTransaction(ProjectOptions options, bool isEarning)
        {
            var projects = projectStorage.Get();
            var project = projects.FirstOrDefault(p => p.Id.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase));
            if (project == null)
            {
                Console.WriteLine("Unknown project " + options.Id);
                return -1;
            }

            if (!double.TryParse(options.Amount.Trim().Replace(',', '.'), CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Failed to parse amount " + options.Amount);
                return -1;
            }

            var summaries = projectSummaryStorage.Get().ToList();
            var summary = summaries.FirstOrDefault(s => s.ProjectId.Equals(options.Id, StringComparison.InvariantCultureIgnoreCase)) 
                         ?? new ProjectSummary(options.Id, 0);
            
            summaries.Remove(summary);
            var previousAmount = summary.Amount;
            var newAmount = isEarning ? summary.Amount + amount : summary.Amount - amount;
            summaries.Add(new ProjectSummary(options.Id, newAmount));
            projectSummaryStorage.Save(summaries);

            var transactionType = isEarning ? "Earned" : "Spent";
            projectPrinter.PrintProjectResult(project, previousAmount, amount, newAmount, transactionType);
            return 0;
        }
    }
}
