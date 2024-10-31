namespace Sibvic.ConsoleMoney.Budget
{
    public interface IBudgetWriter
    {
        void WriteToFile(string fileName, IEnumerable<Budget> budgets);
    }
}