using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class IncomeWriter : IIncomeWriter
    {
        public void WriteToFile(string fileName, IEnumerable<Income> incomes)
        {
            var data = JsonConvert.SerializeObject(incomes.ToArray());
            File.WriteAllText(fileName, data);
        }
    }
}
