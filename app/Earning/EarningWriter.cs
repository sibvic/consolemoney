using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Earning
{
    public class EarningWriter : IEarningWriter
    {
        public void WriteToFile(string filename, IEnumerable<Earning> earnings)
        {
            var data = JsonConvert.SerializeObject(earnings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
