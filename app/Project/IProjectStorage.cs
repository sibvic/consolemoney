namespace Sibvic.ConsoleMoney.Project
{
    public interface IProjectStorage
    {
        Project[] Get();
        void Save(IEnumerable<Project> projects);
    }
}
