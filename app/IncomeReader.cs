using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class IncomeReader : IIncomeReader
    {
        public Income[] ReadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return [];
            }
            var data = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<Income[]?>(data) ?? [];
        }
    }
}
