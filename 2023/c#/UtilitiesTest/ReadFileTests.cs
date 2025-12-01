using Utilities;

namespace UtilitiesTest
{
    [TestClass]
    public class ReadFileTests
    {
        [TestMethod]
        public void Read_Lines_From_Input_File()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input.txt");

            var lines = FileHandling.ReadInputFile(path);

            Assert.AreEqual(13, lines.Count);
            Assert.AreEqual("Test Line 2", lines[1]);
        }
    }
}