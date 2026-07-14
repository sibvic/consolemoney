namespace Sibvic.ConsoleMoney.Budget
{
    public record Budget(string Name, string Id, double? DefaultPercent, bool IsHistoric = false);
}
