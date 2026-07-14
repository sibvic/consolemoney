using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Project
{
    public class ProjectJsonStorage(string homeDir) : IProjectStorage
    {
        string filename = Path.Combine(homeDir, "projects.json");
        public Project[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Project[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Project> projects)
        {
            var data = JsonConvert.SerializeObject(projects.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
