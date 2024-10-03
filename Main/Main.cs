using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

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
            Stemming.stemWord("Ma");
            Console.ReadKey();
        }

        public static void stemLement()
        {
            string suffixes = File.ReadAllText(".\\Files\\suffix.txt");
            string data = File.ReadAllText(".\\Files\\BBC_train_full.csv");

            suffixes = new string((from c in suffixes where c != '-' select c).ToArray());
            string[] suffixList = suffixes.Split('\n');

            //rectify Data
            data = data.Replace('.', ' ');
            data = data.Replace('-', ' ');
            data = new string((from c in data where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());
            CSVData = parseCSVData(data);
            List<List<string>> parsedWords = parseWords(CSVData);

            //remove empty elements
            parsedWords[1].RemoveAll(element => element == "");
            for (int news = 1; news < parsedWords[1].Count - 1; news++)
            {
                //for each word in the first news story
                //for (int i = 0; i < parsedWords[1].Count(); i++)
                //{
                //    Console.CursorLeft = 0;
                //    Console.Write(news);
                //}

                Console.CursorLeft = 0;
                Console.Write(news);
            }
            wordTree = new WordTree(parsedWords[1].ToArray());
            suffixTree = new WordTree(suffixList);
            rSuffixTree = suffixTree.reverseWordTree();
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
            //0 is a constant
            //1 is a vowel

            string returnWord;
            ulong wordStructure = 0;
            char structureLength = (char)word.Count();
            ulong newWordStructure = 0;

            //convert word to word structure 
            for(int i = 0; i < word.Length; i++)
            {
                wordStructure = wordStructure << 1;
                if (!vowels.Contains(word[i]))
                    wordStructure += 1;
            }
            //reduce word structure
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


            Console.WriteLine(Convert.ToString((int)newWordStructure, 2));
            //Count the number of vowel-consonant pairs
            int vcPairs = 0;
            if ((newWordStructure & 1) == 0) newWordStructure >>= 1;
            while (newStructSize > 1)
            {
                if ((newWordStructure & 3) == 1)
                {
                    vcPairs++;
                }
                newWordStructure >>= 2;
                newStructSize -= 2;
            }


            Console.WriteLine(vcPairs);
            return "returnWord";
        }
    }

    public static class Lementization
    {
        public static void lementWord()
        {

        }
    }

}
