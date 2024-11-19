﻿namespace Sibvic.ConsoleMoney.Earning
{
    public record Earning(string IncomeId, DateTime Date, double Amount, double? Rate, string? Comment);
}
