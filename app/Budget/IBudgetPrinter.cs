namespace Sibvic.ConsoleMoney.Budget
{
    public interface IBudgetPrinter
    {
        void Print(IEnumerable<Budget> budgets);
    }
}
