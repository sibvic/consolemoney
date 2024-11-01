using CommandLine;
using Sibvic.ConsoleMoney;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Earning;
using Sibvic.ConsoleMoney.Spending;

Parser.Default.ParseArguments<BudgetOptions, IncomeOptions, SpendingOptions, EarningOptions>(args)
    .MapResult(
        (BudgetOptions opts) => new BudgetController(opts, new BudgetReader(), new BudgetWriter(), new SummaryReader(), new SummaryWriter()).Start(),
        (IncomeOptions opts) => new IncomeController(opts, new IncomeReader(), new IncomeWriter()).Start(),
        (EarningOptions opts) => new EarningController(opts, new EarningReader(), new EarningWriter(), new IncomeReader(), new SummaryReader(), new SummaryWriter(), new BudgetReader()).Start(),
        (SpendingOptions opts) => new SpendingController(opts, new SpendingReader(), new SpendingWriter(), new BudgetReader(), new SummaryReader(), new SummaryWriter()).Start(),
        errs => 1);
