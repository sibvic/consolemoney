namespace Sibvic.ConsoleMoney.Project
{
    public interface IProjectSummaryStorage
    {
        ProjectSummary[] Get();
        void Save(IEnumerable<ProjectSummary> summaries);
    }
}
