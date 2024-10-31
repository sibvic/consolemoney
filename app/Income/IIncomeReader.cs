namespace Sibvic.ConsoleMoney
{
    public interface IIncomeReader
    {
        Income[] ReadFromFile(string filename);
    }
}
