using CommandLine;
using Sibvic.ConsoleMoney;

Parser.Default.ParseArguments<BudgetOptions>(args)
    .MapResult(
        (BudgetOptions opts) => new BudgetController(opts, new BudgetReader(), new BudgetWriter()).Start(),
        errs => 1);
