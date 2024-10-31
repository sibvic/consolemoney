using Moq;
using Sibvic.ConsoleMoney;

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
        }
        Mock<IBudgetReader> budgetReader;
        Mock<IBudgetWriter> budgetWriter;

        [TestMethod]
        public void Add()
        {
            var controller = new BudgetController(new BudgetOptions()
            {
                Add = true,
                Name = "name",
                Id = "n"
            }, budgetReader.Object, budgetWriter.Object);
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(0, controller.Start());
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Budget>>(items =>
                items.Count() == 1 && items.First().Name == "name" && items.First().Id == "n")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            var controller = new BudgetController(new BudgetOptions()
            {
                Add = true,
                Name = "",
                Id = "n"
            }, budgetReader.Object, budgetWriter.Object);
            budgetReader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(-1, controller.Start());
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Budget>>()), Times.Never);

            controller = new BudgetController(new BudgetOptions()
            {
                Add = true,
                Name = "name",
                Id = ""
            }, budgetReader.Object, budgetWriter.Object);
            Assert.AreEqual(-1, controller.Start());
            budgetWriter.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Budget>>()), Times.Never);
        }
    }
}