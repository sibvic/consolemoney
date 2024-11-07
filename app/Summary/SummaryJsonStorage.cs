using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class SummaryJsonStorage(string homeDir) : ISummaryStorage
    {
        string filename = Path.Combine("summaries.json", homeDir);

        public Summary[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Summary[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Summary> earnings)
        {
            var data = JsonConvert.SerializeObject(earnings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
