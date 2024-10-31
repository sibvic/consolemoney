using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class BudgetReader : IBudgetReader
    {
        public Budget[]? ReadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return [];
            }
            var data = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<Budget[]?>(data) ?? [];
        }
    }
}
