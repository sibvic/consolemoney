using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sibvic.ConsoleMoney.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTests
{
    [TestClass]
    public class ProjectControllerTest
    {
        private Mock<IProjectStorage> _projectStorageMock;
        private Mock<IProjectSummaryStorage> _projectSummaryStorageMock;
        private Mock<IProjectPrinter> _projectPrinterMock;
        private ProjectController _controller;

        [TestInitialize]
        public void Setup()
        {
            _projectStorageMock = new Mock<IProjectStorage>();
            _projectSummaryStorageMock = new Mock<IProjectSummaryStorage>();
            _projectPrinterMock = new Mock<IProjectPrinter>();
            _controller = new ProjectController(_projectStorageMock.Object, _projectSummaryStorageMock.Object, _projectPrinterMock.Object);
        }

        [TestMethod]
        public void AddProject_WithValidData_ShouldSucceed()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Add = true,
                Id = "test-id"
            };
            var existingProjects = new List<Project>();
            _projectStorageMock.Setup(x => x.Get()).Returns(existingProjects.ToArray());

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(0, result);
            _projectStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<Project>>()), Times.Once);
            _projectPrinterMock.Verify(x => x.Print(It.IsAny<Project[]>(), It.IsAny<ProjectSummary[]>()), Times.Once);
        }

        [TestMethod]
        public void AddProject_WithMissingId_ShouldFail()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Add = true,
                Id = ""
            };

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(-1, result);
            _projectStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<Project>>()), Times.Never);
        }

        [TestMethod]
        public void AddProject_WithDuplicateId_ShouldFail()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Add = true,
                Id = "test-id"
            };
            var existingProjects = new List<Project> { new Project("test-id") };
            _projectStorageMock.Setup(x => x.Get()).Returns(existingProjects.ToArray());

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(-1, result);
            _projectStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<Project>>()), Times.Never);
        }

        [TestMethod]
        public void ShowProjects_ShouldCallPrinter()
        {
            // Arrange
            var options = new ProjectOptions { Show = true };
            var projects = new Project[] { new Project("test-id") };
            _projectStorageMock.Setup(x => x.Get()).Returns(projects);

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(0, result);
            _projectPrinterMock.Verify(x => x.Print(projects, new ProjectSummary[0]), Times.Once);
        }

        [TestMethod]
        public void SpendFromProject_WithValidData_ShouldSucceed()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Spend = true,
                Id = "test-id",
                Amount = "100.50"
            };
            var projects = new Project[] { new("test-id") };
            var summaries = new List<ProjectSummary> { new("test-id", 200.0) };
            _projectStorageMock.Setup(x => x.Get()).Returns(projects);
            _projectSummaryStorageMock.Setup(x => x.Get()).Returns(summaries.ToArray());

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(0, result);
            _projectSummaryStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<ProjectSummary>>()), Times.Once);
            _projectPrinterMock.Verify(x => x.PrintProjectResult(
                It.IsAny<Project>(), 
                It.IsAny<double>(), 
                It.IsAny<double>(), 
                It.IsAny<double>(), 
                "Spent"), Times.Once);
        }

        [TestMethod]
        public void EarnIntoProject_WithValidData_ShouldSucceed()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Earn = true,
                Id = "test-id",
                Amount = "150.75"
            };
            var projects = new Project[] { new Project("test-id") };
            var summaries = new List<ProjectSummary> { new ProjectSummary("test-id", 100.0) };
            _projectStorageMock.Setup(x => x.Get()).Returns(projects);
            _projectSummaryStorageMock.Setup(x => x.Get()).Returns(summaries.ToArray());

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(0, result);
            _projectSummaryStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<ProjectSummary>>()), Times.Once);
            _projectPrinterMock.Verify(x => x.PrintProjectResult(
                It.IsAny<Project>(), 
                It.IsAny<double>(), 
                It.IsAny<double>(), 
                It.IsAny<double>(), 
                "Earned"), Times.Once);
        }

        [TestMethod]
        public void SpendFromProject_WithInvalidAmount_ShouldFail()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Spend = true,
                Id = "test-id",
                Amount = "invalid"
            };
            var projects = new Project[] { new Project( "test-id") };
            _projectStorageMock.Setup(x => x.Get()).Returns(projects);

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(-1, result);
            _projectSummaryStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<ProjectSummary>>()), Times.Never);
        }

        [TestMethod]
        public void SpendFromProject_WithUnknownProject_ShouldFail()
        {
            // Arrange
            var options = new ProjectOptions
            {
                Spend = true,
                Id = "unknown-id",
                Amount = "100"
            };
            var projects = new Project[] { new Project("test-id") };
            _projectStorageMock.Setup(x => x.Get()).Returns(projects);

            // Act
            var result = _controller.Start(options);

            // Assert
            Assert.AreEqual(-1, result);
            _projectSummaryStorageMock.Verify(x => x.Save(It.IsAny<IEnumerable<ProjectSummary>>()), Times.Never);
        }
    }
}
