using CommandLine;
using Sibvic.ConsoleMoney;

Parser.Default.ParseArguments<BudgetOptions, IncomeOptions>(args)
    .MapResult(
        (BudgetOptions opts) => new BudgetController(opts, new BudgetReader(), new BudgetWriter()).Start(),
        (IncomeOptions opts) => new IncomeController(opts, new IncomeReader(), new IncomeWriter()).Start(),
        errs => 1);
