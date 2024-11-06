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
            reader = new Mock<IEarningReader>();
            writer = new Mock<IEarningWriter>();
            incomeReader = new Mock<IIncomeReader>();
            summaryReader = new Mock<ISummaryReader>();
            summaryWriter = new Mock<ISummaryWriter>();
            budgetReader = new Mock<IBudgetReader>();
        }
        Mock<IIncomeReader> incomeReader;
        Mock<IEarningReader> reader;
        Mock<IEarningWriter> writer;
        Mock<ISummaryReader> summaryReader;
        Mock<ISummaryWriter> summaryWriter;
        Mock<IBudgetReader> budgetReader;

        EarningController Create()
        {
            return new EarningController(reader.Object, writer.Object, incomeReader.Object, summaryReader.Object, summaryWriter.Object, budgetReader.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            reader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null)]);
            incomeReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                new IncomeDistribushing("car", 15),
                new IncomeDistribushing("", 1),
                ])]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns(
                [
                    new Budget.Budget("invest_", "invest"),
                    new Budget.Budget("car_", "car"),
                    new Budget.Budget("coffee", "coffee")
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = 100,
                IncomeId = "main"
            }));
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 2
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Amount == 200
                && items.ElementAt(1).IncomeId == "main" && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Amount == 100
            )));
            summaryWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 3
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 45
                && items.ElementAt(1).BudgetId == "car" && items.ElementAt(1).Amount == 15
                && items.ElementAt(2).BudgetId == "coffee" && items.ElementAt(2).Amount == 1
            )));
        }

        [TestMethod]
        public void AddWithRate()
        {
            var controller = Create();
            reader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([]);
            incomeReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Income("main income", "main", [
                new IncomeDistribushing("invest", 10),
                ])]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("invest", 35)]);
            budgetReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns(
                [
                    new Budget.Budget("invest_", "invest")
                ]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Amount = 100,
                IncomeId = "main",
                Rate = "1.78"
            }));
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Earning.Earning>>(items =>
                items.Count() == 1
                && items.ElementAt(0).IncomeId == "main" && items.ElementAt(0).Date == DateTime.Now.Date && items.ElementAt(0).Amount == 100 && items.ElementAt(0).Rate == 1.78
            )));
            summaryWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1
                && items.ElementAt(0).BudgetId == "invest" && items.ElementAt(0).Amount == 52.8
            )));
        }

        [TestMethod]
        public void AddUnknonwIncome()
        {
            var controller = Create();
            reader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Earning.Earning("main", new DateTime(2000, 1, 1), 200, null)]);
            incomeReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Income("main income", "main", [])]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Amount = 100,
                IncomeId = "main2"
            }));
        }
    }
}