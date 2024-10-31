using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class BudgetWriter : IBudgetWriter
    {
        public void WriteToFile(string fileName, IEnumerable<Budget> budgets)
        {
            var data = JsonConvert.SerializeObject(budgets.ToArray());
            File.WriteAllText(fileName, data);
        }
    }
}
