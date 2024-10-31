namespace Sibvic.ConsoleMoney.Spending
{
    public interface ISpendingReader
    {
        Spending[] ReadFromFile(string filename);
    }
}