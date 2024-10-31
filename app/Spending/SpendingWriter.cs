using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Spending
{
    public class SpendingWriter : ISpendingWriter
    {
        public void WriteToFile(string filename, IEnumerable<Spending> spendings)
        {
            var data = JsonConvert.SerializeObject(spendings.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
