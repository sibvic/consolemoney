namespace Sibvic.ConsoleMoney.Budget
{
    public interface IBudgetReader
    {
        Budget[]? ReadFromFile(string filename);
    }
}