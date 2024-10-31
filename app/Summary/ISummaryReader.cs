namespace Sibvic.ConsoleMoney
{
    public interface ISummaryReader
    {
        Summary[]? ReadFromFile(string filename);
    }
}