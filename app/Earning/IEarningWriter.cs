namespace Sibvic.ConsoleMoney.Earning
{
    public interface IEarningWriter
    {
        void WriteToFile(string filename, IEnumerable<Earning> earnings);
    }
}
