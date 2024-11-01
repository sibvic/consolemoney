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
            reader = new Mock<ISpendingReader>();
            writer = new Mock<ISpendingWriter>();
            budgetReader = new Mock<IBudgetReader>();
            summaryReader = new Mock<ISummaryReader>();
            summaryWriter = new Mock<ISummaryWriter>();
        }
        Mock<ISpendingReader> reader;
        Mock<ISpendingWriter> writer;
        Mock<IBudgetReader> budgetReader;
        Mock<ISummaryReader> summaryReader;
        Mock<ISummaryWriter> summaryWriter;

        SpendingController Create(SpendingOptions options)
        {
            return new SpendingController(options, reader.Object, writer.Object, budgetReader.Object, summaryReader.Object, summaryWriter.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create(new()
            {
                BudgetId = "main",
                Comment = "coffee",
                Amount = "150.05"
            });
            budgetReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Budget.Budget("", "main")]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("main", 450)]);
            reader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([new Spending.Spending(new DateTime(2000, 1, 1), "test", "main", 123.45)]);

            Assert.AreEqual(0, controller.Start());
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Spending.Spending>>(items =>
                items.Count() == 2 
                && items.ElementAt(0).Date == new DateTime(2000, 1, 1) && items.ElementAt(0).Comment == "test" && items.ElementAt(0).BudgetId == "main" && items.ElementAt(0).Amount == 123.45
                && items.ElementAt(1).Date == DateTime.Now.Date && items.ElementAt(1).Comment == "coffee" && items.ElementAt(1).BudgetId == "main" && items.ElementAt(1).Amount == 150.05)));
            summaryWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1 
                && items.First().Amount == 299.95
            )));
        }

        [TestMethod]
        public void AddUnknownBudget()
        {
            var controller = Create(new()
            {
                BudgetId = "main2",
                Comment = "coffee",
                Amount = "150.05"
            });
            budgetReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Budget.Budget("", "main")]);
            reader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([new Spending.Spending(new DateTime(2000, 1, 1), "test", "main", 123.45)]);

            Assert.AreEqual(-1, controller.Start());
        }
    }
}