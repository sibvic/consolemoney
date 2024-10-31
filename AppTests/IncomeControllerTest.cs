using Moq;

namespace Sibvic.ConsoleMoney.AppTests
{
    [TestClass]
    public class IncomeControllerTest
    {
        [TestInitialize]
        public void Init()
        {
            reader = new Mock<IIncomeReader>();
            writer = new Mock<IIncomeWriter>();
        }
        Mock<IIncomeReader> reader;
        Mock<IIncomeWriter> writer;

        [TestMethod]
        public void Add()
        {
            var controller = new IncomeController(new()
            {
                Add = true,
                Name = "name",
                Id = "n"
            }, reader.Object, writer.Object);
            reader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(0, controller.Start());
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.Is<IEnumerable<Income>>(items =>
                items.Count() == 1 && items.First().Name == "name" && items.First().Id == "n")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            var controller = new IncomeController(new()
            {
                Add = true,
                Name = "",
                Id = "n"
            }, reader.Object, writer.Object);
            reader.Setup(c => c.ReadFromFile(It.IsAny<string>())).Returns([]);

            Assert.AreEqual(-1, controller.Start());
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Income>>()), Times.Never);

            controller = new IncomeController(new()
            {
                Add = true,
                Name = "name",
                Id = ""
            }, reader.Object, writer.Object);
            Assert.AreEqual(-1, controller.Start());
            writer.Verify(w => w.WriteToFile(It.IsAny<string>(), It.IsAny<IEnumerable<Income>>()), Times.Never);
        }
    }
}