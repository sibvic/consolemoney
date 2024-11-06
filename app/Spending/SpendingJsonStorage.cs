using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingJsonStorage : ISpendingStorage
    {
        const string filename = "spendings.json";
        public Spending[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Spending[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Spending> earnings)
        {
            var data = JsonConvert.SerializeObject(earnings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
