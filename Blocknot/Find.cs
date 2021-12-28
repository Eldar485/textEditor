using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocknot
{
    public class Find
    {
        public List<int> Finds(string text, string findText)
        {
            var list = new List<int>();
            int lastFind = 0;
            bool next = true;
            while (next)
            {
                if (text.IndexOf(findText, lastFind) >= 0)
                {
                    list.Add(text.IndexOf(findText, lastFind));
                    lastFind = text.IndexOf(findText, lastFind) + findText.Length;
                }
                else
                    next = false;
            }
            return list;
        }
    }
}
