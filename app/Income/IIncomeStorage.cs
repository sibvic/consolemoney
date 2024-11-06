namespace Sibvic.ConsoleMoney
{
    public interface IIncomeStorage
    {
        Income[] Get();
        void Save(IEnumerable<Income> budgets);
    }
}
