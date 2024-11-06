using Moq;
using Sibvic.ConsoleMoney.Budget;

namespace Sibvic.ConsoleMoney.AppTests
{

    [TestClass]
    public class BudgetControllerTest
    {
        [TestInitialize]
        public void Init()
        {
            budgetReader = new Mock<IBudgetReader>();
            budgetWriter = new Mock<IBudgetWriter>();
            summaryReader = new Mock<ISummaryReader>();
            summaryWriter = new Mock<ISummaryWriter>();
        }
        Mock<IBudgetReader> budgetReader;
        Mock<IBudgetWriter> budgetWriter;
        Mock<ISummaryReader> summaryReader;
        Mock<ISummaryWriter> summaryWriter;

        BudgetController Create()
        {
            return new BudgetController(budgetReader.Object, budgetWriter.Object, summaryReader.Object, summaryWriter.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = "n"
            }));
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Budget.Budget>>(items =>
                items.Count() == 1 && items.First().Name == "name" && items.First().Id == "n")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            var controller = Create();
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "",
                Id = "n"
            }));
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Budget.Budget>>()), Times.Never);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = ""
            }));
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Budget.Budget>>()), Times.Never);
        }

        [TestMethod]
        public void SetAmount()
        {
            var controller = Create();
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "z",
                InitialAmount = 100.15
            }));
            summaryWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 2
                && items.ElementAt(0).BudgetId == "x" && items.ElementAt(0).Amount == 15.20
                && items.ElementAt(1).BudgetId == "z" && items.ElementAt(1).Amount == 100.15)));
        }

        [TestMethod]
        public void SetAmountUnknown()
        {
            var controller = Create();
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "c",
                InitialAmount = 100.15
            }));
        }

        [TestMethod]
        public void SetAmountDuplicate()
        {
            var controller = Create();
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.ReadFromFile(It.IsAny<string>())).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "x",
                InitialAmount = 100.15
            }));
        }
    }
}