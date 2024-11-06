using Moq;
using Sibvic.ConsoleMoney.Budget;
using Sibvic.ConsoleMoney.Spending;

namespace Sibvic.ConsoleMoney.AppTests
{
    [TestClass]
    public class SpendingControllerTest
    {
        [TestInitialize]
        public void Init()
        {
            reader = new Mock<ISpendingStorage>();
            budgetReader = new Mock<IBudgetStorage>();
            summaryReader = new Mock<ISummaryStorage>();
        }
        Mock<ISpendingStorage> reader;
        Mock<IBudgetStorage> budgetReader;
        Mock<ISummaryStorage> summaryReader;

        SpendingController Create()
        {
            return new SpendingController(reader.Object, budgetReader.Object, summaryReader.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("main", 450)]);
            reader.Setup(c => c.Get()).Returns([new Spending.Spending(new DateTime(2000, 1, 1), "test", "main", 123.45)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                BudgetId = "main",
                Comment = "coffee",
                Amount = "150.05"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Spending.Spending>>(items =>
                items.Count() == 2 
                && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Comment == "test" && items.ElementAt(0).BudgetId == "main" && items.ElementAt(0).Amount == 123.45
                && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Comment == "coffee" && items.ElementAt(1).BudgetId == "main" && items.ElementAt(1).Amount == 150.05)));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1 
                && items.First().Amount == 299.95
            )));
        }

        [TestMethod]
        public void AddComma()
        {
            var controller = Create();
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("main", 450)]);
            reader.Setup(c => c.Get()).Returns([new Spending.Spending(new DateTime(2000, 1, 1), "test", "main", 123.45)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                BudgetId = "main",
                Comment = "coffee",
                Amount = "150,05"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Spending.Spending>>(items =>
                items.Count() == 2
                && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Comment == "test" && items.ElementAt(0).BudgetId == "main" && items.ElementAt(0).Amount == 123.45
                && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Comment == "coffee" && items.ElementAt(1).BudgetId == "main" && items.ElementAt(1).Amount == 150.05)));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1
                && items.First().Amount == 299.95
            )));
        }

        [TestMethod]
        public void AddUnknownBudget()
        {
            var controller = Create();
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main")]);
            reader.Setup(c => c.Get()).Returns([new Spending.Spending(new DateTime(2000, 1, 1), "test", "main", 123.45)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                BudgetId = "main2",
                Comment = "coffee",
                Amount = "150.05"
            }));
        }
    }
}