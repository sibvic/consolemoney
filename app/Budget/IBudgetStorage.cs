namespace Sibvic.ConsoleMoney.Budget
{
    public interface IBudgetStorage
    {
        Budget[] Get();
        void Save(IEnumerable<Budget> budgets);
    }
}