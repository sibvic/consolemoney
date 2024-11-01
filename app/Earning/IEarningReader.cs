namespace Sibvic.ConsoleMoney.Earning
{
    public interface IEarningReader
    {
        Earning[] ReadFromFile(string filename);
    }
}
