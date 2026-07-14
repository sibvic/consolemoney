using CommandLine;
using Sibvic.ConsoleMoney;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Earning;
using Sibvic.ConsoleMoney.Project;
using Sibvic.ConsoleMoney.Spending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var homeDir = OptionsStorage.GetHomeDir(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", ""));

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IBudgetStorage>(new BudgetJsonStorage(homeDir));
builder.Services.AddSingleton<ISummaryStorage>(new SummaryJsonStorage(homeDir));
builder.Services.AddSingleton<IIncomeStorage>(new IncomeJsonStorage(homeDir));
builder.Services.AddSingleton<IEarningStorage>(new EarningJsonStorage(homeDir));
builder.Services.AddSingleton<ISpendingStorage>(new SpendingJsonStorage(homeDir));
builder.Services.AddSingleton<IProjectStorage>(new ProjectJsonStorage(homeDir));
builder.Services.AddSingleton<IProjectSummaryStorage>(new ProjectSummaryJsonStorage(homeDir));
builder.Services.AddTransient<IBudgetPrinter, ConsoleBudgetPrinter>();
builder.Services.AddTransient<IEarningsPrinter, ConsoleEarningsPrinter>();
builder.Services.AddTransient<ISpendingPrinter, ConsoleSpendingPrinter>();
builder.Services.AddTransient<IIncomeSummaryPrinter, ConsoleIncomeSummaryPrinter>();
builder.Services.AddTransient<IProjectPrinter, ConsoleProjectPrinter>();
builder.Services.AddTransient<BudgetController>();
builder.Services.AddTransient<IncomeController>();
builder.Services.AddTransient<EarningController>();
builder.Services.AddTransient<SpendingController>();
builder.Services.AddTransient<ProjectController>();
using IHost host = builder.Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

Parser.Default.ParseArguments<BudgetOptions, IncomeOptions, SpendingOptions, EarningOptions, ProjectOptions>(args)
    .MapResult(
        (BudgetOptions opts) => provider.GetService<BudgetController>().Start(opts),
        (IncomeOptions opts) => provider.GetService<IncomeController>().Start(opts),
        (EarningOptions opts) => provider.GetService<EarningController>().Start(opts),
        (SpendingOptions opts) => provider.GetService<SpendingController>().Start(opts),
        (ProjectOptions opts) => provider.GetService<ProjectController>().Start(opts),
        errs => 1);
