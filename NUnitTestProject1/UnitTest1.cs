using NUnit.Framework;
using Blocknot;
using System.Windows;

namespace Blocknot
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Count()
        {
            Form1 form1 = new Form1();
            int result = form1.Count1("Алло asdasd a sd  s");
            string s = System.IO.File.ReadAllText(@"fileName.txt").Replace("\n", " ");

            Assert.AreEqual(5, result);
        }
    }
}