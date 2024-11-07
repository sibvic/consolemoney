using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Earning
{
    public class EarningJsonStorage(string homeDir) : IEarningStorage
    {
        string filename = Path.Combine(homeDir, "earnings.json");

        public Earning[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Earning[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Earning> earnings)
        {
            var data = JsonConvert.SerializeObject(earnings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
