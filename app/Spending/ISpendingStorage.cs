namespace Sibvic.ConsoleMoney.Spending
{
    public interface ISpendingStorage
    {
        Spending[] Get();
        void Save(IEnumerable<Spending> budgets);
    }
}
