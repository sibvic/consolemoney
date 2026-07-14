using CommandLine;

namespace Sibvic.ConsoleMoney.Project
{
    [Verb("project", HelpText = "Project management.")]
    public class ProjectOptions
    {
        [Option('a', "add", HelpText = "Add a project.")]
        public bool Add { get; set; }
        [Option('s', "show", HelpText = "Show list of projects.")]
        public bool Show { get; set; }
        [Option("spend", HelpText = "Spend amount from project.")]
        public bool Spend { get; set; }
        [Option("earn", HelpText = "Earn amount into project.")]
        public bool Earn { get; set; }
        [Option("amount", HelpText = "Amount.")]
        public string? Amount { get; set; }
        [Option('i', "id", HelpText = "Project id")]
        public string Id { get; set; }
        [Option('c', "comment", HelpText = "Comment for the transaction")]
        public string? Comment { get; set; }
    }
}
