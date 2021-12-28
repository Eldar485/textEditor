using NUnit.Framework;
using System.Collections.Generic;

namespace Blocknot
{
    public class Tests
    {
        [Test]
        public void CountWords()
        {
            CountWord countWord = new CountWord();
            Assert.AreEqual(countWord.CountWords("asdsd sdasd w3123 ghfgh"), 4);
            Assert.AreEqual(countWord.CountWords("awdwd-wdawd"), 1);
            Assert.AreEqual(countWord.CountWords(""), 0);
            Assert.AreEqual(countWord.CountWords("asdsd ; sdasd $ w3123 ! ghfgh"), 7);
        }
        [Test]
        public void CountChar()
        {
            CountCh countCh = new CountCh();
            Assert.AreEqual(countCh.CountChars("asd"), 3);
            Assert.AreEqual(countCh.CountChars("aw-wd"), 5);
            Assert.AreEqual(countCh.CountChars(""), 0);
            Assert.AreEqual(countCh.CountChars("asd sas"), 7);
        }
        [Test]
        public void CountParagraphs()
        {
            CountParagraphs countParagraphs = new CountParagraphs();
            Assert.AreEqual(countParagraphs.CountParagraph("asd"), false);
            Assert.AreEqual(countParagraphs.CountParagraph("aw-wd"), false);
            Assert.AreEqual(countParagraphs.CountParagraph(""), false);
            Assert.AreEqual(countParagraphs.CountParagraph("  "), false);
            Assert.AreEqual(countParagraphs.CountParagraph("   asdasdsd"), true);
            Assert.AreEqual(countParagraphs.CountParagraph("        sadasd"), true);
            Assert.AreEqual(countParagraphs.CountParagraph("    "), true);
        }
        [Test]
        public void Find()
        {
            Find find = new Find();
            int[] expected1 = new int[] { 1 };
            IEnumerable<int> processed1 = find.Finds("sasssssssss", "a");
            CollectionAssert.AreEqual(processed1, expected1);
            int[] expected2 = new int[] { 0 };
            IEnumerable<int> processed2 = find.Finds("assssssssss", "a");
            CollectionAssert.AreEqual(processed2, expected2);
            int[] expected3 = new int[] { 10 };
            IEnumerable<int> processed3 = find.Finds("ssssssssssa", "a");
            CollectionAssert.AreEqual(processed3, expected3);
            int[] expected4 = new int[] { 1, 2 };
            IEnumerable<int> processed4 = find.Finds("saassssssss", "a");
            CollectionAssert.AreEqual(processed4, expected4);
            int[] expected5 = new int[] { 1, 6 };
            IEnumerable<int> processed5 = find.Finds("sassssassss", "a");
            CollectionAssert.AreEqual(processed5, expected5);
            int[] expected6 = new int[] { 0, 10 };
            IEnumerable<int> processed6 = find.Finds("asssssssssa", "a");
            CollectionAssert.AreEqual(processed6, expected6);
            int[] expected7 = new int[] { 1 };
            IEnumerable<int> processed7 = find.Finds("saassssssss", "aa");
            CollectionAssert.AreEqual(processed7, expected7);
            int[] expected8 = new int[] { };
        }
    }
}