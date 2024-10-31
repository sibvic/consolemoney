using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingReader : ISpendingReader
    {
        public Spending[] ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Spending[]?>(data) ?? [];
        }
    }
}
