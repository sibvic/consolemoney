using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sibvic.ConsoleMoney.Earning;
using System;
using System.IO;

namespace Sibvic.ConsoleMoney.AppTests
{
    [TestClass]
    public class EarningsPrinterTest
    {
        private StringWriter consoleOutput;
        private TextWriter originalConsoleOut;
        private ConsoleEarningsPrinter printer;

        [TestInitialize]
        public void Init()
        {
            originalConsoleOut = Console.Out;
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            printer = new ConsoleEarningsPrinter();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(originalConsoleOut);
            consoleOutput.Dispose();
        }

        [TestMethod]
        public void PrintLastNEarnings_ShowsCorrectNumberOfEarnings()
        {
            var earnings = new[]
            {
                new Earning.Earning("main", new DateTime(2024, 3, 20), 1000, 1.0, "March salary"),
                new Earning.Earning("freelance", new DateTime(2024, 3, 15), 500, null, "Project X"),
                new Earning.Earning("consulting", new DateTime(2024, 3, 10), 750, 1.25, "Client Y"),
                new Earning.Earning("bonus", new DateTime(2024, 3, 5), 300, null, "Q1 bonus"),
                new Earning.Earning("main", new DateTime(2024, 3, 1), 1000, 1.0, "February salary")
            };

            printer.PrintLastNEarnings(earnings, 3);

            var output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("Last 3 earnings:"));
            Assert.IsTrue(output.Contains("Date"));
            Assert.IsTrue(output.Contains("2024-03-20"));
            Assert.IsTrue(output.Contains("2024-03-15"));
            Assert.IsTrue(output.Contains("2024-03-10"));
            Assert.IsFalse(output.Contains("2024-03-05"));
            Assert.IsFalse(output.Contains("2024-03-01"));
        }

        [TestMethod]
        public void PrintLastNEarnings_EmptyList_ShowsNoEarningsMessage()
        {
            printer.PrintLastNEarnings(Array.Empty<Earning.Earning>(), 5);

            var output = consoleOutput.ToString();
            Assert.AreEqual("No earnings found." + Environment.NewLine, output);
        }

        [TestMethod]
        public void PrintLastNEarnings_FormatsAllFieldsCorrectly()
        {
            var earnings = new[]
            {
                new Earning.Earning("main", new DateTime(2024, 3, 20), 1000.50, 1.25, "Test comment")
            };

            printer.PrintLastNEarnings(earnings, 1);

            var output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("Date"));
            Assert.IsTrue(output.Contains("2024-03-20"));
            Assert.IsTrue(output.Contains(1000.50.ToString("F2")));
            Assert.IsTrue(output.Contains("main"));
            Assert.IsTrue(output.Contains(1.25.ToString("F2")));
            Assert.IsTrue(output.Contains("Test comment"));
        }

        [TestMethod]
        public void PrintLastNEarnings_HandlesNullValues()
        {
            var earnings = new[]
            {
                new Earning.Earning("main", new DateTime(2024, 3, 20), 1000.50, null, null)
            };

            printer.PrintLastNEarnings(earnings, 1);

            var output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("Date"));
            Assert.IsTrue(output.Contains("2024-03-20"));
            Assert.IsTrue(output.Contains(1000.50.ToString("F2")));
            Assert.IsTrue(output.Contains("main"));
            Assert.IsTrue(output.Contains("N/A"));
        }
    }
}
