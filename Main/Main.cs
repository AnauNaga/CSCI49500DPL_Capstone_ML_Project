using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

            //for each word in the first news story
            for (int i = 0; i < parsedWords[1].Count();i++)
            {
                Console.Write(parsedWords[1][i]);
                Console.Write(' ');
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
        public static string[] stemArray(string[] words)
        {
            string returnArray[];

            return returnArray;

        }

        public static string stemWord(string word)
        {
            string returnWord;
            char[] wordArray = word.ToArray();
            wordArray;
            return returnWord;
        }
    }

    public static class Lementization
    {
        public static void lementWord()
        {

        }
    }

}
