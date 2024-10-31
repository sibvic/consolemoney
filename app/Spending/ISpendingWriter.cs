namespace Sibvic.ConsoleMoney.Spending
{
    public interface ISpendingWriter
    {
        void WriteToFile(string filename, IEnumerable<Spending> spendings);
    }
}