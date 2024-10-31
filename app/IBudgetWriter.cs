
namespace Sibvic.ConsoleMoney
{
    public interface IBudgetWriter
    {
        void WriteToFile(string fileName, IEnumerable<Budget> budgets);
    }
}