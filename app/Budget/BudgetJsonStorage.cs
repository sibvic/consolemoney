using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney.Budget
{
    public class BudgetJsonStorage(string homeDir) : IBudgetStorage
    {
        string filename = Path.Combine("budgets.json", homeDir);

        public Budget[] Get()
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Budget[]?>(data) ?? [];
        }

        public void Save(IEnumerable<Budget> budgets)
        {
            var data = JsonConvert.SerializeObject(budgets.ToArray());
            File.WriteAllText(filename, data);
        }
    }
}