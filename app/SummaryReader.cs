using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class SummaryReader : ISummaryReader
    {
        public Summary[]? ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Summary[]?>(data) ?? [];
        }
    }
}