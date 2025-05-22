using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sibvic.ConsoleMoney.Earning;
using System;
using System.IO;
using System.Linq;

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
                new Earning("main", new DateTime(2024, 3, 20), 1000, 1.0, "March salary"),
                new Earning("freelance", new DateTime(2024, 3, 15), 500, null, "Project X"),
                new Earning("consulting", new DateTime(2024, 3, 10), 750, 1.25, "Client Y"),
                new Earning("bonus", new DateTime(2024, 3, 5), 300, null, "Q1 bonus"),
                new Earning("main", new DateTime(2024, 3, 1), 1000, 1.0, "February salary")
            };

            printer.PrintLastNEarnings(earnings, 3);

            var output = consoleOutput.ToString();
            var lines = output.Split(Environment.NewLine);

            Assert.IsTrue(lines[0].Contains("Last 3 earnings:"));
            Assert.IsTrue(lines[1].Contains("Date")); // Header row
            Assert.IsTrue(lines[2].Contains("2024-03-20")); // Most recent
            Assert.IsTrue(lines[3].Contains("2024-03-15")); // Second most recent
            Assert.IsTrue(lines[4].Contains("2024-03-10")); // Third most recent
        }

        [TestMethod]
        public void PrintLastNEarnings_EmptyList_ShowsNoEarningsMessage()
        {
            printer.PrintLastNEarnings(Array.Empty<Earning>(), 5);

            var output = consoleOutput.ToString();
            Assert.AreEqual("No earnings found." + Environment.NewLine, output);
        }

        [TestMethod]
        public void PrintLastNEarnings_FormatsAllFieldsCorrectly()
        {
            var earnings = new[]
            {
                new Earning("main", new DateTime(2024, 3, 20), 1000.50, 1.25, "Test comment")
            };

            printer.PrintLastNEarnings(earnings, 1);

            var output = consoleOutput.ToString();
            var lines = output.Split(Environment.NewLine);

            Assert.IsTrue(lines[1].Contains("Date")); // Header row
            Assert.IsTrue(lines[2].Contains("2024-03-20")); // Date
            Assert.IsTrue(lines[2].Contains("1000.50")); // Amount
            Assert.IsTrue(lines[2].Contains("main")); // Income ID
            Assert.IsTrue(lines[2].Contains("1.25")); // Rate
            Assert.IsTrue(lines[2].Contains("Test comment")); // Comment
        }

        [TestMethod]
        public void PrintLastNEarnings_HandlesNullValues()
        {
            var earnings = new[]
            {
                new Earning("main", new DateTime(2024, 3, 20), 1000.50, null, null)
            };

            printer.PrintLastNEarnings(earnings, 1);

            var output = consoleOutput.ToString();
            var lines = output.Split(Environment.NewLine);

            Assert.IsTrue(lines[1].Contains("Date")); // Header row
            Assert.IsTrue(lines[2].Contains("2024-03-20")); // Date
            Assert.IsTrue(lines[2].Contains("1000.50")); // Amount
            Assert.IsTrue(lines[2].Contains("main")); // Income ID
            Assert.IsTrue(lines[2].Contains("N/A")); // Rate
            Assert.IsTrue(lines[2].Contains("")); // Empty comment
        }
    }
} 