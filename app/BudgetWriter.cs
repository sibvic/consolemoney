using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class BudgetWriter : IBudgetWriter
    {
        public void WriteToFile(string filename, IEnumerable<Budget> budgets)
        {
            var data = JsonConvert.SerializeObject(budgets.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}
