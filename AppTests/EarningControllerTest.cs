using Moq;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Earning;
using System.Net.Http.Headers;

namespace Sibvic.ConsoleMoney.AppTests
{
    [TestClass]
    public class EarningControllerTest
    {
        [TestInitialize]
        public void Init()
        {
            reader = new Mock<IEarningStorage>();
            incomeReader = new Mock<IIncomeStorage>();
            summaryReader = new Mock<ISummaryStorage>();
            budgetReader = new Mock<IBudgetStorage>();
            budgetPrinter = new Mock<IBudgetPrinter>();
        }
        Mock<IIncomeStorage> incomeReader;
        Mock<IEarningStorage> reader;
        Mock<ISummaryStorage> summaryReader;
        Mock<IBudgetStorage> budgetReader;
        Mock<IBudgetPrinter> budgetPrinter;

        EarningController Create()
        {
            return new EarningController(reader.Object, incomeReader.Object, summaryReader.Object,
                budgetReader.Object, budgetPrinter.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null, null)]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                new IncomeDistribushing("car", 15),
                new IncomeDistribushing("", 1),
                ])]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.Get()).Returns(
                [
                    new Budget.Budget("invest_", "invest", null),
                    new Budget.Budget("car_", "car", null),
                    new Budget.Budget("coffee", "coffee", null)
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = "100",
                IncomeId = "main",
                Comment = "test"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 2
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Amount == 200
                && items.ElementAt(1).IncomeId == "main" && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Amount == 100 && items.ElementAt(1).Comment == "test"
            )));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 3
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 45
                && items.ElementAt(1).BudgetId == "car" && items.ElementAt(1).Amount == 15
                && items.ElementAt(2).BudgetId == "coffee" && items.ElementAt(2).Amount == 1
            )));
        }

        [TestMethod]
        public void AddDefaultPercent()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null, null)]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                new IncomeDistribushing("car", 15),
                new IncomeDistribushing("", 1),
                ])]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.Get()).Returns(
                [
                    new Budget.Budget("invest_", "invest", null),
                    new Budget.Budget("car_", "car", null),
                    new Budget.Budget("coffee", "coffee", 0.01)
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = "100",
                IncomeId = "main",
                Comment = "test"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 2
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Amount == 200
                && items.ElementAt(1).IncomeId == "main" && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Amount == 100 && items.ElementAt(1).Comment == "test"
            )));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 3
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 45
                && items.ElementAt(1).BudgetId == "car" && items.ElementAt(1).Amount == 15
                && items.ElementAt(2).BudgetId == "coffee" && items.ElementAt(2).Amount == 0.01
            )));
        }

        [TestMethod]
        public void AddBadAmount()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null, null)]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                new IncomeDistribushing("car", 15),
                new IncomeDistribushing("", 1),
                ])]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.Get()).Returns(
                [
                    new Budget.Budget("invest_", "invest", null),
                    new Budget.Budget("car_", "car", null),
                    new Budget.Budget("coffee", "coffee", null)
                ]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Amount = "100z",
                IncomeId = "main"
            }));
        }

        [TestMethod]
        public void AddWithRate()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                ])]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.Get()).Returns(
                [
                    new Budget.Budget("invest_", "invest", null)
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = "100",
                IncomeId = "main",
                Rate = "1.78"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 1
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == DateTime.Now.Date && items.ElementAt(0).Amount == 100 && items.ElementAt(0).Rate == 1.78
            )));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 52.8
            )));
        }

        [TestMethod]
        public void AddWithRateComma()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                ])]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.Get()).Returns(
                [
                    new Budget.Budget("invest_", "invest", null)
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = "100",
                IncomeId = "main",
                Rate = "1,78"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 1
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == DateTime.Now.Date && items.ElementAt(0).Amount == 100 && items.ElementAt(0).Rate == 1.78
            )));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 52.8
            )));
        }

        [TestMethod]
        public void AddUnknonwIncome()
        {
            var controller = Create();
            reader.Setup(r => r.Get()).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null, null)]);
            incomeReader.Setup(r => r.Get()).Returns([new Income("main income", "main", [])]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Amount = "100",
                IncomeId = "main2"
            }));
        }
    }
}