namespace Sibvic.ConsoleMoney
{
    public record IncomeDistribushing(string BudgetId, double Percent);
    public record Income(string Name, string Id, IncomeDistribushing[] Distribushings);
}
