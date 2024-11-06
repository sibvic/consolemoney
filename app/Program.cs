using CommandLine;
using Sibvic.ConsoleMoney;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Earning;
using Sibvic.ConsoleMoney.Spending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<IBudgetStorage, BudgetJsonStorage>();
builder.Services.AddTransient<ISummaryStorage, SummaryJsonStorage>();
builder.Services.AddTransient<IIncomeStorage, IncomeJsonStorage>();
builder.Services.AddTransient<IEarningStorage, EarningJsonStorage>();
builder.Services.AddTransient<ISpendingStorage, SpendingJsonStorage>();
builder.Services.AddTransient<BudgetController>();
builder.Services.AddTransient<IncomeController>();
builder.Services.AddTransient<EarningController>();
builder.Services.AddTransient<SpendingController>();
using IHost host = builder.Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

Parser.Default.ParseArguments<BudgetOptions, IncomeOptions, SpendingOptions, EarningOptions>(args)
    .MapResult(
        (BudgetOptions opts) => provider.GetService<BudgetController>().Start(opts),
        (IncomeOptions opts) => provider.GetService<IncomeController>().Start(opts),
        (EarningOptions opts) => provider.GetService<EarningController>().Start(opts),
        (SpendingOptions opts) => provider.GetService<SpendingController>().Start(opts),
        errs => 1);
