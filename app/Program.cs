using CommandLine;
using Sibvic.ConsoleMoney;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Spending;

Parser.Default.ParseArguments<BudgetOptions, IncomeOptions, SpendingOptions>(args)
    .MapResult(
        (BudgetOptions opts) => new BudgetController(opts, new BudgetReader(), new BudgetWriter(), new SummaryReader(), new SummaryWriter()).Start(),
        (IncomeOptions opts) => new IncomeController(opts, new IncomeReader(), new IncomeWriter()).Start(),
        (SpendingOptions opts) => new SpendingController(opts, new SpendingReader(), new SpendingWriter(), new BudgetReader(), new SummaryReader(), new SummaryWriter()).Start(),
        errs => 1);
