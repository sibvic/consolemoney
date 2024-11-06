namespace Sibvic.ConsoleMoney
{
    public interface ISummaryStorage
    {
        Summary[] Get();
        void Save(IEnumerable<Summary> budgets);
    }
}
