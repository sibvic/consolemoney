namespace Sibvic.ConsoleMoney
{
    public class IncomeController(IncomeOptions options, IIncomeReader incomeReader, IIncomeWriter incomeWriter)
    {
        public int Start()
        {
            if (options.Add)
            {
                if (string.IsNullOrEmpty(options.Name) || string.IsNullOrEmpty(options.Id))
                {
                    Console.WriteLine("Income name and id should be specified");
                    return -1;
                }
                var incomes = incomeReader.ReadFromFile("incomes.json").ToList();
                incomes.Add(new Income(options.Name, options.Id));
                incomeWriter.WriteToFile("incomes.json", incomes);
                return 0;
            }
            if (options.Show)
            {
                var incomes = incomeReader.ReadFromFile("Incomes.json").ToList();
                Console.WriteLine("List of incomes:");
                foreach (var income in incomes)
                {
                    Console.WriteLine("- " + income.Name + "(" + income.Id + ")");
                }
            }
            return 0;
        }
    }
}
