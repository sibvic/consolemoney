using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class IncomeReader : IIncomeReader
    {
        public Income[] ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Income[]?>(data) ?? [];
        }
    }
}
