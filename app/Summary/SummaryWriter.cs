using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class SummaryWriter : ISummaryWriter
    {
        public void WriteToFile(string filename, IEnumerable<Summary> summaries)
        {
            var data = JsonConvert.SerializeObject(summaries.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
