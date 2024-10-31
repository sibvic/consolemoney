
namespace Sibvic.ConsoleMoney
{
    public interface ISummaryWriter
    {
        void WriteToFile(string filename, IEnumerable<Summary> summaries);
    }
}