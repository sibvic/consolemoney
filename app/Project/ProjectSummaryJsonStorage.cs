using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Project
{
    public class ProjectSummaryJsonStorage(string homeDir) : IProjectSummaryStorage
    {
        string filename = Path.Combine(homeDir, "project_summaries.json");

        public ProjectSummary[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<ProjectSummary[]?>(data) ?? [];
        }

        public void Save(IEnumerable<ProjectSummary> summaries)
        {
            var data = JsonConvert.SerializeObject(summaries.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
