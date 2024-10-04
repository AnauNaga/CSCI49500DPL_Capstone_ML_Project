using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public static class Stemmer
    {
        public static Dictionary<string, string> suffixDictOne = new Dictionary<string, string>();
        public static Dictionary<string, string> suffixDictTwo = new Dictionary<string, string>();
        public static Dictionary<string, string> suffixDictThree = new Dictionary<string, string>();

        public static List<string> stemArray(List<string> words)
        {
            List<string> returnArray = new List<string>();

            for (int i = 0; i < words.Count; i++)
            {
            }

            return returnArray;

        }

        private static char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        public static string stemWord(string word)
        {
            //bit packing
            //0 is a vowel
            //1 is a consonant

            string returnWord = "";
            WordStructure.initWordStructure(word);
            returnWord = word;

            //
            //      Step 1a
            //
            //remove plurality
            if (word.EndsWith("sses")) returnWord = replaceEnd(word, "sses", "ss");
            else if (word.EndsWith("ies")) returnWord = replaceEnd(word, "ies", "i");
            else if (word.EndsWith("ss")) returnWord = replaceEnd(word, "ss", "ss");
            else if (word.EndsWith("s")) returnWord = replaceEnd(word, "s", "");

            //
            //      Step 1b
            //

            //remove temporality
            bool removedTemporality = false;
            if (word.EndsWith("eed")) { if (WordStructure.vcPairs > 1) returnWord = replaceEnd(word, "eed", "ee"); }
            else if (word.EndsWith("ed"))
            {
                string replacement = replaceEnd(word, "ed", "");
                if (replacement.FirstOrDefault(letter => vowels.Contains(letter)) != 0) { returnWord = replacement; removedTemporality = true; }
            }
            else if (word.EndsWith("ing"))
            {
                string replacement = replaceEnd(word, "ing", "");
                if (replacement.FirstOrDefault(letter => vowels.Contains(letter)) != 0) { returnWord = replacement; removedTemporality = true; }
            }

            //If the word should've had a vowel ending, add it back
            char[] exceptionLetters = { 'w', 'x', 'y' };
            char[] doubleLetters = { 'l', 's', 'z' };
            if (removedTemporality)
            {

                if (returnWord.EndsWith("at")) returnWord += "e";
                else if (returnWord.EndsWith("bl")) returnWord += "e";
                else if (returnWord.EndsWith("iz")) returnWord += "e";
                else if ((returnWord.Last() == returnWord[returnWord.Count() - 2]))
                {
                    if (!doubleLetters.Contains(returnWord.Last()))
                    {
                        returnWord = returnWord.Remove(returnWord.Count() - 1);
                    }
                }
                else if ((WordStructure.vcPairs == 2) && (((WordStructure.structure >> (WordStructure.structureLength - 3)) & 7) == 5) && (!exceptionLetters.Contains(returnWord.Last())))
                    returnWord += "e";
            }

            //
            //      Step 1c
            //
            //replace y with i
            if ((WordStructure.vcPairs > 0) && !vowels.Contains(returnWord[returnWord.Count() - 2]) && returnWord.Last() == 'y')
                returnWord = returnWord.Remove(returnWord.Count() - 1) + 'i';

            //
            //      Step 2
            //
            WordStructure.initWordStructure(returnWord);
            if (WordStructure.vcPairs > 2)
            {
                string currWordEnding = "";
                string suffixCandidate = "";
                for (int i = returnWord.Length - 1; i >= 0; i--)
                {
                    currWordEnding = returnWord[i] + currWordEnding;
                    if (suffixDictOne.ContainsKey(currWordEnding))
                        suffixCandidate = currWordEnding;
                }
                if (suffixCandidate.Length > 0)
                    returnWord = replaceEnd(returnWord, suffixCandidate, suffixDictOne[suffixCandidate]);
            }

            //
            //      Step 3
            //
            WordStructure.initWordStructure(returnWord);
            if (WordStructure.vcPairs > 2)
            {
                string currWordEnding = "";
                string suffixCandidate = "";
                for (int i = returnWord.Length - 1; i >= 0; i--)
                {
                    currWordEnding = returnWord[i] + currWordEnding;
                    if (suffixDictTwo.ContainsKey(currWordEnding))
                        suffixCandidate = currWordEnding;
                }
                if (suffixCandidate.Length > 0)
                    returnWord = replaceEnd(returnWord, suffixCandidate, suffixDictTwo[suffixCandidate]);
            }

            //
            //      Step 4
            //
            WordStructure.initWordStructure(returnWord);
            if (WordStructure.vcPairs > 3)
            {
                string currWordEnding = "";
                string suffixCandidate = "";
                for (int i = returnWord.Length - 1; i >= 0; i--)
                {
                    currWordEnding = returnWord[i] + currWordEnding;
                    if (suffixDictThree.ContainsKey(currWordEnding))
                        suffixCandidate = currWordEnding;
                }
                if (suffixCandidate.Length > 0)
                    returnWord = replaceEnd(returnWord, suffixCandidate, suffixDictThree[suffixCandidate]);
            }

            //
            //      Step 5a
            //
            //We need the updated word structure for returnWord
            WordStructure.initWordStructure(returnWord);

            if ((WordStructure.vcPairs > 1) && returnWord.EndsWith("e")) returnWord = returnWord.Remove(returnWord.Length - 1);
            if ((WordStructure.vcPairs == 1) && WordStructure.beginsWithC && returnWord.EndsWith("e") &&
                !exceptionLetters.Contains(returnWord[returnWord.Length - 2]))
                returnWord = returnWord.Remove(returnWord.Length - 1);

            //
            //      Step 5b
            //
            if ((WordStructure.vcPairs > 1) && (returnWord[returnWord.Length - 1] == 'l') && (returnWord.EndsWith("l")))
                returnWord = returnWord.Remove(returnWord.Length - 1);

            return returnWord;
        }

        public static string replaceEnd(string word, string suffix, string newEnd)
        {
            string newWord;
            int wordLength = word.Count();
            //remove suffix
            newWord = word.Remove(word.Length - suffix.Length);

            //apply new end
            newWord += newEnd;

            return newWord;
        }

        private static class WordStructure
        {


            public static ulong structure;
            public static short structureLength;
            public static ulong reducedStructure;
            public static short reducedLength;

            public static int vcPairs = 0;
            public static bool beginsWithC = false;
            public static bool endsWithV = false;

            public static void initWordStructure(string word)
            {
                structure = 0;                    //bit packed word
                structureLength = (short)word.Count();
                reducedStructure = 0;                 //bit packed word

                //convert word to word structure 
                for (int i = 0; i < word.Length; i++)
                {
                    structure = structure << 1;
                    if (!vowels.Contains(word[i]))
                        structure += 1;

                }

                //reduce word structure
                ulong tempStructure = structure;
                char currLetter = (char)(structure & 1);
                tempStructure = tempStructure >> 1;
                reducedLength = 1;
                for (int i = 0; i < word.Length - 1; i++)
                {
                    //if there are two consonants or vowels in a row, combine them
                    reducedStructure = reducedStructure | currLetter;
                    if (currLetter == (char)(tempStructure & 1))
                    {
                        tempStructure >>= 1;
                    }
                    else
                    {
                        reducedLength++;
                        reducedStructure = reducedStructure << 1;
                        currLetter = (char)(tempStructure & 1);
                        tempStructure >>= 1;
                    }
                }
                reducedStructure = reducedStructure | currLetter;

                //Count the number of vowel-consonant pairs
                vcPairs = 0;
                beginsWithC = false;
                endsWithV = false;

                //newWordStructure is orientated backwards from wordStructure
                //The left most value of word structure is the first letter of the word
                //while the right most of newWordStructure is the first letter

                //if the first letter begins with a constant, note it down.
                if ((reducedStructure & 1) == 1) { reducedStructure >>= 1; reducedLength--; beginsWithC = true; }
                //if the last letter ends with a vowel, note it down.
                if ((reducedStructure & ((ulong)1 << (reducedLength - 1))) == 0) endsWithV = true;

                while (reducedLength > 1)
                {
                    if ((reducedStructure & 3) == 2)
                    {
                        vcPairs++;
                    }
                    reducedStructure >>= 2;
                    reducedLength -= 2;
                }

            }
        }
    }
}
