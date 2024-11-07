using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class IncomeJsonStorage(string homeDir) : IIncomeStorage
    {
        string filename = Path.Combine("incomes.json", homeDir);

        public Income[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Income[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Income> earnings)
        {
            var data = JsonConvert.SerializeObject(earnings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
