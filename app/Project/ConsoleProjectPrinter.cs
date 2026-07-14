using Alba.CsConsoleFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Project
{
    public class ConsoleProjectPrinter : IProjectPrinter
    {
        public void Print(Project[] projects, ProjectSummary[] summaries)
        {
            if (projects.Length == 0)
            {
                Console.WriteLine("No projects found.");
                return;
            }

            var doc = new Document();
            var table = new Grid
            {
                Stroke = LineThickness.Single,
                StrokeColor = ConsoleColor.Gray
            };

            table.Columns.Add(GridLength.Auto);
            table.Columns.Add(GridLength.Auto);

            table.Children.Add(new Cell("Id") { Stroke = LineThickness.Single });
            table.Children.Add(new Cell("Amount") { Stroke = LineThickness.Single });

            foreach (var project in projects)
            {
                table.Children.Add(new Cell(project.Id) { Stroke = LineThickness.Single });
                table.Children.Add(new Cell(summaries.Where(s => s.ProjectId.Equals(project.Id, StringComparison.InvariantCultureIgnoreCase))
                            .Select(s => s.Amount)
                            .FirstOrDefault(0).ToString("n0")) { Stroke = LineThickness.Single });
            }

            doc.Children.Add(table);
            ConsoleDocumentRenderer.Render(doc);
        }

        public void PrintProjectResult(Project project, double previousAmount, double transactionAmount, double newAmount, string transactionType)
        {
            Console.WriteLine($"{transactionType} from/to project {project.Id}:");
            Console.WriteLine($"  Previous amount: {previousAmount:F2}");
            Console.WriteLine($"  {transactionType} amount: {transactionAmount:F2}");
            Console.WriteLine($"  New amount: {newAmount:F2}");
        }
    }
}
