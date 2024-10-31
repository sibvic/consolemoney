namespace Sibvic.ConsoleMoney
{
    public interface IIncomeWriter
    {
        void WriteToFile(string filename, IEnumerable<Income> incomes);
    }
}
