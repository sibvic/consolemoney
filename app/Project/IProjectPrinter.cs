namespace Sibvic.ConsoleMoney.Project
{
    public interface IProjectPrinter
    {
        void Print(Project[] projects, ProjectSummary[] summaries);
        void PrintProjectResult(Project project, double previousAmount, double transactionAmount, double newAmount, string transactionType);
    }
}
