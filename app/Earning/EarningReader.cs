using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Earning
{
    public class EarningReader : IEarningReader
    {
        public Earning[] ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Earning[]?>(data) ?? [];
        }
    }
}
