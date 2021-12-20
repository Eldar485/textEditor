using NUnit.Framework;
using Blocknot;

namespace Blocknot
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Copy()
        {
            Form1 form1 = new Form1();
            string result = form1.SaveFile("����");
            Assert.AreEqual("����", result, "Account not debited correctly");
        }
    }
}