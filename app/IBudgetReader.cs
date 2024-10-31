namespace Sibvic.ConsoleMoney
{
    public interface IBudgetReader
    {
        Budget[]? ReadFromFile(string filename);
    }
}