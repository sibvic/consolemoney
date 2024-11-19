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
            budgetReader = new Mock<IBudgetStorage>();
            summaryReader = new Mock<ISummaryStorage>();
            budgetPrinter = new Mock<IBudgetPrinter>();
        }
        Mock<IBudgetStorage> budgetReader;
        Mock<ISummaryStorage> summaryReader;
        Mock<IBudgetPrinter> budgetPrinter;

        BudgetController Create()
        {
            return new BudgetController(budgetReader.Object, summaryReader.Object, budgetPrinter.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = "n"
            }));
            budgetReader.Verify(w => w.Save(It.Is<IEnumerable<Budget.Budget>>(items =>
                items.Count() == 1 && items.First().Name == "name" && items.First().Id == "n")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "",
                Id = "n"
            }));
            budgetReader.Verify(w => w.Save(It.IsAny<IEnumerable<Budget.Budget>>()), Times.Never);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = ""
            }));
            budgetReader.Verify(w => w.Save(It.IsAny<IEnumerable<Budget.Budget>>()), Times.Never);
        }

        [TestMethod]
        public void TopUp()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                TopUp= true,
                Id = "x",
                Amount = "100.15"
            }));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 1
                && items.ElementAt(0).BudgetId == "x" && Math.Abs(items.ElementAt(0).Amount - 115.35) < 0.01)));
        }

        [TestMethod]
        public void SetAmount()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "z",
                Amount = "100.15"
            }));
            summaryReader.Verify(w => w.Save(It.Is<IEnumerable<Summary>>(items =>
                items.Count() == 2
                && items.ElementAt(0).BudgetId == "x" && items.ElementAt(0).Amount == 15.20
                && items.ElementAt(1).BudgetId == "z" && items.ElementAt(1).Amount == 100.15)));
        }

        [TestMethod]
        public void SetAmountUnknown()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "c",
                Amount = "100.15"
            }));
        }

        [TestMethod]
        public void SetAmountDuplicate()
        {
            var controller = Create();
            budgetReader.Setup(c => c.Get()).Returns([new Budget.Budget("", "z"), new Budget.Budget("", "x")]);
            summaryReader.Setup(r => r.Get()).Returns([new Summary("x", 15.20)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetInitialAmount = true,
                Id = "x",
                Amount = "100.15"
            }));
        }
    }
}