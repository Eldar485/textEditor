using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocknot
{
    public class CountParagraphs
    {
        public bool CountParagraph(string text)
        {
            if (text.IndexOf("   ") == 0)
                return true;
            return false;
        }
    }
}
