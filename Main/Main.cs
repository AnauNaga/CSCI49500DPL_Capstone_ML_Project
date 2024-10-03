using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Main
{
    public static class Master
    {

        //words share a common letter starting from their beginning
        public static WordTree wordTree;
        public static WordTree suffixTree;

        //words share a common letter starting from their end
        public static WordTree rSuffixTree;
        public static List<List<string>> CSVData;

        public static void Main(string[] args)
        {
            string suffixes = File.ReadAllText(".\\Files\\suffix.txt");
            string data = File.ReadAllText(".\\Files\\BBC_train_full.csv");
            string stopWords = File.ReadAllText(".\\Files\\stopWords.txt");


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
            for (int i = 0; i < parsedWords[1].Count();i++)
            {
                Console.Write(parsedWords[1][i]);
                Console.Write(' ');
            }
            wordTree = new WordTree(parsedWords[1].ToArray());
            suffixTree = new WordTree(suffixList);
            rSuffixTree = suffixTree.reverseWordTree();
            List<string> line = parsedWords[1];
            List<string> stemmedLine = Stemming.stemLine(line);

            Console.WriteLine();
            foreach(string word in stemmedLine)
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
            List<List<string>> returnData = new List<List<string>>();
            for (int category = 0; category < Categories.Length; category++) returnData.Add(new List<string>());
            
            for (int row = 0; row < rows.Length; row++)
            {
                string[] rowSplit = rows[row].Split(',');
                //Add News Category
                returnData[0].Add(""); 
                returnData[0][row] = rowSplit[0];
                //Add News Text
                returnData[1].Add("");
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
        public static List<string> stemLine(List<string> line)
        {
            List<string> stemmedLine = new List<string>(line.Count);


            for(int i = 0; i < line.Count; i++)
            {
                stemmedLine.Add("");
                stemmedLine[i] = stemWord(line[i]);

            }

            return stemmedLine;
        }

        public static string stemWord(string word)
        {
            int length = word.Length;
            string stemmedWord = word;
            WordTree.Tree currBranch = Master.rSuffixTree.wordTree;

            for(int letter = length - 1; letter >= 0; letter--)
            {
                char currletter = word[letter];

                //if the next letter exists in the tree
                if (currBranch.branches.ContainsKey(currletter))
                {
                    stemmedWord =  stemmedWord.TrimEnd(currletter);
                    currBranch = currBranch.branches[currletter];
                }
                else if (currBranch.isWord )
                {
                    return stemmedWord;
                }

                
            }
            return null;
        }
    }

    public static class Lementization
    {
        public static void lementWord()
        {

        }
    }

}
