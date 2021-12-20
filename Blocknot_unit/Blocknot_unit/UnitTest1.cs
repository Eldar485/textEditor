using NUnit.Framework;
using System.Collections.Generic;

namespace Blocknot
{
    public class Tests
    {
        [Test]
        public void CountWords()
        {
            Block a = new Block();
            Assert.AreEqual(a.CountWords("asdsd sdasd w3123 ghfgh"), 4);
            Assert.AreEqual(a.CountWords("awdwd-wdawd"), 1);
            Assert.AreEqual(a.CountWords(""), 0);
            Assert.AreEqual(a.CountWords("asdsd ; sdasd $ w3123 ! ghfgh"), 7);
        }
        [Test]
        public void CountChar()
        {
            Block a = new Block();
            Assert.AreEqual(a.CountChar("asd"), 3);
            Assert.AreEqual(a.CountChar("aw-wd"), 5);
            Assert.AreEqual(a.CountChar(""), 0);
            Assert.AreEqual(a.CountChar("asd sas"), 7);
        }
        [Test]
        public void CountParagraphs()
        {
            Block a = new Block();
            Assert.AreEqual(a.CountParagraphs("asd"), false);
            Assert.AreEqual(a.CountParagraphs("aw-wd"), false);
            Assert.AreEqual(a.CountParagraphs(""), false);
            Assert.AreEqual(a.CountParagraphs("  "), false);
            Assert.AreEqual(a.CountParagraphs("   asdasdsd"), true);
            Assert.AreEqual(a.CountParagraphs("        sadasd"), true);
            Assert.AreEqual(a.CountParagraphs("    "), true);
        }
        [Test]
        public void Find()
        {
            Block a = new Block();
            int[] expected1 = new int[] { 1 };
            IEnumerable<int> processed1 = a.Find("sasssssssss", "a");
            CollectionAssert.AreEqual(processed1, expected1);
            int[] expected2 = new int[] { 0 };
            IEnumerable<int> processed2 = a.Find("assssssssss", "a");
            CollectionAssert.AreEqual(processed2, expected2);
            int[] expected3 = new int[] { 10 };
            IEnumerable<int> processed3 = a.Find("ssssssssssa", "a");
            CollectionAssert.AreEqual(processed3, expected3);
            int[] expected4 = new int[] { 1, 2 };
            IEnumerable<int> processed4 = a.Find("saassssssss", "a");
            CollectionAssert.AreEqual(processed4, expected4);
            int[] expected5 = new int[] { 1, 6 };
            IEnumerable<int> processed5 = a.Find("sassssassss", "a");
            CollectionAssert.AreEqual(processed5, expected5);
            int[] expected6 = new int[] { 0, 10 };
            IEnumerable<int> processed6 = a.Find("asssssssssa", "a");
            CollectionAssert.AreEqual(processed6, expected6);
            int[] expected7 = new int[] { 1 };
            IEnumerable<int> processed7 = a.Find("saassssssss", "aa");
            CollectionAssert.AreEqual(processed7, expected7);
            int[] expected8 = new int[] { };
        }
      
    }
}