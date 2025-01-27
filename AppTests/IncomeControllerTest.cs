﻿using Moq;
using Sibvic.ConsoleMoney.Budget;

namespace Sibvic.ConsoleMoney.AppTests
{
    [TestClass]
    public class IncomeControllerTest
    {
        [TestInitialize]
        public void Init()
        {
            reader = new Mock<IIncomeStorage>();
            budgetReader = new Mock<IBudgetStorage>();
        }
        Mock<IIncomeStorage> reader;
        Mock<IBudgetStorage> budgetReader;

        IncomeController Create()
        {
            return new IncomeController(reader.Object, budgetReader.Object);
        }

        [TestMethod]
        public void Add()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([]);

            Assert.AreEqual(0, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = "n"
            }));
            reader.Verify(w => w.Save(It.Is<IEnumerable<Income>>(items =>
                items.Count() == 1 && items.First().Name == "name" && items.First().Id == "n")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "",
                Id = "n"
            }));
            reader.Verify(w => w.Save(It.IsAny<IEnumerable<Income>>()), Times.Never);

            Assert.AreEqual(-1, controller.Start(new()
            {
                Add = true,
                Name = "name",
                Id = ""
            }));
            reader.Verify(w => w.Save(It.IsAny<IEnumerable<Income>>()), Times.Never);
        }

        [TestMethod]
        public void SetDistribution()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([new Income("", "n", [])]);
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main", null)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                SetDistribution = true,
                Id = "n",
                BudgetId = "main",
                DistributionPercent = "14.1"
            }));

            reader.Verify(w => w.Save(It.Is<IEnumerable<Income>>(items =>
                items.Count() == 1 && items.First().Id == "n" && items.First().Distribushings.Length == 1
                && items.First().Distribushings[0].BudgetId == "main"
                && items.First().Distribushings[0].Percent == 14.1)));
        }

        [TestMethod]
        public void SetDistributionComma()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([new Income("", "n", [])]);
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main", null)]);

            Assert.AreEqual(0, controller.Start(new()
            {
                SetDistribution = true,
                Id = "n",
                BudgetId = "main",
                DistributionPercent = "14,1"
            }));

            reader.Verify(w => w.Save(It.Is<IEnumerable<Income>>(items =>
                items.Count() == 1 && items.First().Id == "n" && items.First().Distribushings.Length == 1
                && items.First().Distribushings[0].BudgetId == "main"
                && items.First().Distribushings[0].Percent == 14.1)));
        }

        [TestMethod]
        public void SetDistributionUnknownIncome()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([new Income("", "n", [])]);
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main", null)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetDistribution = true,
                Id = "m",
                BudgetId = "main",
                DistributionPercent = "14.1"
            }));
        }

        [TestMethod]
        public void SetDistributionUnknownBudget()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([new Income("", "n", [])]);
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main", null)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetDistribution = true,
                Id = "n",
                BudgetId = "main2",
                DistributionPercent = "14.1"
            }));
        }

        [TestMethod]
        public void SetDistributionBadPercent()
        {
            var controller = Create();
            reader.Setup(c => c.Get()).Returns([new Income("", "n", [])]);
            budgetReader.Setup(r => r.Get()).Returns([new Budget.Budget("", "main", null)]);

            Assert.AreEqual(-1, controller.Start(new()
            {
                SetDistribution = true,
                Id = "n",
                BudgetId = "main",
                DistributionPercent = "1z4.1"
            }));
        }
    }
}