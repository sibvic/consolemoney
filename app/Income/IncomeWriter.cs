using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class IncomeWriter : IIncomeWriter
    {
        public void WriteToFile(string filename, IEnumerable<Income> incomes)
        {
            var data = JsonConvert.SerializeObject(incomes.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
