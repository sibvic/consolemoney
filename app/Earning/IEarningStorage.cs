namespace Sibvic.ConsoleMoney.Earning
{
    public interface IEarningStorage
    {
        Earning[] Get();
        void Save(IEnumerable<Earning> budgets);
    }
}
