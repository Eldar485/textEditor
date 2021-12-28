using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocknot
{
    public class CountWord
    {
        enum WordCountState
        {
            Init,
            Word,
            WhiteSpace
        }
        public int CountWords(string originString)
        {
            int wordCounter = 0;
            WordCountState state = WordCountState.Init;

            foreach (Char c in originString)
            {
                // In case of whitespace
                if (Char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case WordCountState.Init:
                        case WordCountState.Word:
                            state = WordCountState.WhiteSpace;
                            break;

                        case WordCountState.WhiteSpace:
                            // ignore whitespace chars
                            break;

                        default:
                            throw new InvalidProgramException();
                    }
                    // In case of non-whitespace char
                }
                else
                {
                    switch (state)
                    {
                        case WordCountState.Init:
                        case WordCountState.WhiteSpace:
                            // Incerement out counter if we met non-whitespace
                            // char after whitespace (one or more)
                            wordCounter++;
                            state = WordCountState.Word;
                            break;

                        case WordCountState.Word:
                            // ignore all symbols in word
                            break;

                        default:
                            throw new InvalidProgramException();
                    }
                }
            }

            return wordCounter;
        }
    }
}
