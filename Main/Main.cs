using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using LemmaSharp;

namespace Main
{
    public static class Master
    {
        public static WordTree wordTree;
        public static WordTree suffixTree;
        public static WordTree rSuffixTree;
        public static List<List<string>> CSVData;

        public static void Main(string[] args)
        {
            string input = "";
            while (input != "end")
            {
                input = Console.ReadLine();
                Console.WriteLine("Stemmed word:");
                Console.WriteLine(Stemming.stemWord(input));
            }



            //Stemming.stemWord("equitize");
            //Console.ReadKey();
        }

        public static void stemLement()
        {
            //read given files
            string suffixes     = File.ReadAllText(".\\Files\\suffix.txt");
            string data         = File.ReadAllText(".\\Files\\BBC_train_full.csv");
            string stopWords    = File.ReadAllText(".\\Files\\stopWords.txt");


            //remove the return character
            suffixes = new string((from c in suffixes where (c != '-') select c).ToArray());
            suffixes = suffixes.Replace("\r\n", "\n");
            string[] suffixList = suffixes.Split('\n');

            stopWords = stopWords.Replace("\r\n", "\n");
            string[] stopWordList = stopWords.Split('\n');

            //rectify Data
            data = data.Replace('.', ' ');
            data = data.Replace('-', ' ');
            data = new string((from c in data where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());
            CSVData = parseCSVData(data);
            List<List<string>> parsedWords = parseWords(CSVData);

            //remove empty elements
            parsedWords[1].RemoveAll(element => element == "");
            //remove stop words
            parsedWords[1].RemoveAll(element => stopWordList.Contains(element));
            //for each word in the first news story
            for (int i = 0; i < parsedWords[1].Count(); i++)
            {
                Console.Write(parsedWords[1][i]);
                Console.Write(' ');
            }
            wordTree = new WordTree(parsedWords[1].ToArray());
            suffixTree = new WordTree(suffixList);
            rSuffixTree = suffixTree.reverseWordTree();
            List<string> line = parsedWords[1];
            List<string> stemmedLine = Stemming.stemArray(line);
            Console.WriteLine();
            foreach (string word in stemmedLine)
            {
                Console.Write(word + ' ');
            }

            Console.ReadKey();
        }

        public static List<List<string>> parseCSVData(string data)
        {
            //Split the CSV file
            string[] rows = data.Split('\n');
            string[] Categories = rows[0].Split(',');

            //initialize 2D array
            List<List<string>> returnData = new List<List<string>>(Categories.Length) ;
            for (int i = 0;i < Categories.Length;i++) returnData[i] = new List<string>(rows.Length);
            
            for (int row = 0; row < rows.Length; row++)
            {
                string[] rowSplit = rows[row].Split(',');
                //Add News Category
                returnData[0][row] = rowSplit[0];
                //Add News Text
                returnData[1][row] = rowSplit[1];
            }

            return returnData;
        }

        public static List<List<string>> parseWords(List<List<string>> CSVData)
        {
            List<List<string>> parsedWords = new List<List<string>>();
            for(int row = 0; row < CSVData[1].Count(); row++)
            {
                parsedWords.Add(new List<string>());
                parsedWords[row] = CSVData[1][row].Split(' ').ToList();
            }

            return parsedWords;
        }
    }

    
    public static class Stemming
    {
        public static List<string> stemArray(List<string> words)
        {
            List<string> returnArray = new List<string>();

            for (int i = 0; i < words.Count; i++) {
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
            ulong wordStructure = 0;                    //bit packed word
            char structureLength = (char)word.Count();
            ulong newWordStructure = 0;                 //bit packed word

            //convert word to word structure 
            for (int i = 0; i < word.Length; i++)
            {
                wordStructure = wordStructure << 1;
                if (!vowels.Contains(word[i]))
                    wordStructure += 1;

            }
            //reduce word structure
            ulong oldWordStructure = wordStructure;
            char currLetter = (char)(wordStructure & 1);
            wordStructure = wordStructure >> 1;
            int newStructSize = 1;
            for (int i = 0; i < word.Length-1; i++)
            {
                //if there are two consonants or vowels in a row, combine them
                newWordStructure = newWordStructure | currLetter;
                if ( currLetter == (char)(wordStructure & 1))
                {
                    wordStructure >>= 1;
                }
                else
                {
                    newStructSize++;
                    newWordStructure = newWordStructure << 1;
                    currLetter = (char)(wordStructure & 1);
                    wordStructure >>= 1;
                }
            }
            newWordStructure = newWordStructure | currLetter;
            //Count the number of vowel-consonant pairs
            int vcPairs = 0;
            bool beginsWithC = false;
            bool endsWithV = false;

            //newWordStructure is orientated backwards from wordStructure
            //The left most value of word structure is the first letter of the word
            //while the right most of newWordStructure is the first letter

            //if the first letter begins with a constant, note it down.
            if ((newWordStructure & 1) == 1) { newWordStructure >>= 1; newStructSize--; beginsWithC = true; }
            //if the last letter ends with a vowel, note it down.
            if ((newWordStructure & ((ulong)1 << (newStructSize-1))) == 0) endsWithV = true;

            while (newStructSize > 1)
            {
                if ((newWordStructure & 3) == 2)
                {
                    vcPairs++;
                }
                newWordStructure >>= 2;
                newStructSize -= 2;
            }
            returnWord = word;

            //remove plurality
            if (word.EndsWith("sses")) returnWord = replaceEnd(word, "sses", "ss");
            else if (word.EndsWith("ies")) returnWord = replaceEnd(word, "ies", "i");
            else if (word.EndsWith("ss")) returnWord = replaceEnd(word, "ss", "ss");
            else if (word.EndsWith("s")) returnWord = replaceEnd(word, "s", "");

            //remove temporality
            bool removedTemporality = false;
            if (word.EndsWith("eed")) { if (vcPairs > 1) returnWord = replaceEnd(word, "eed", "ee"); }
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
            if (removedTemporality)
            {
                char[] exceptionLetters = { 'w', 'x', 'y' };
                char[] doubleLetters = { 'l', 's', 'z' };

                if (returnWord.EndsWith("at")) returnWord += "e";
                else if (returnWord.EndsWith("bl")) returnWord += "e";
                else if (returnWord.EndsWith("iz")) returnWord += "e";
                else if ((returnWord.Last() == returnWord[returnWord.Count() - 2])){
                    if (!doubleLetters.Contains(returnWord.Last())){
                        returnWord = returnWord.Remove(returnWord.Count() - 1);
                    }
                }
                else if ((vcPairs == 2) && (((oldWordStructure>>(structureLength - 3)) & 7) == 5) && (!exceptionLetters.Contains(returnWord.Last())))
                    returnWord += "e";
            }

            //put the y back in a word if it needs it
            if((vcPairs > 0) && !vowels.Contains(returnWord[returnWord.Count()-2]) && returnWord.Last() == 'y')
                returnWord = returnWord.Remove(returnWord.Count()-1) + 'i';
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

    }

    public static class Lementization
    {
        public static Lemmatizer lemmatizer = new Lemmatizer();
        public static List<string> lementArray(List<string> words)
        {
            List<string> returnList = new List<string>();
            foreach (string word in words)
            {
                returnList.Add(lemmatizer.Lemmatize(word));
            }
            return returnList;
        }

        public static void lementWord()
        {

        }
    }

}
