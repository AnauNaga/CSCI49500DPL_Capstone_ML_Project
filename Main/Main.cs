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

        public static List<List<string>> CSVData;
        public static List<List<string>> stemmedWords;
        public static List<List<string>> lemmentedWords;

        public static void Main(string[] args)
        {
            //List<string> vocabulary = new List<string>();
            //stemLemment();
            //foreach(List<string> newsStory in lemmentedWords)
            //    foreach (string word in newsStory) 
            //        if(!vocabulary.Contains(word))vocabulary.Add(word);

            //foreach (string word in vocabulary)
            //    Console.WriteLine(word);
            string input = "";
            while (input != "end")
            {
                input = Console.ReadLine();
                Console.WriteLine(Lemmenter.lemmentWord(input));
            }

            Console.ReadKey();
        }

        public static void stemLemment()
        {
            //read given files
            string suffixesOne     = File.ReadAllText(".\\Files\\suffix1.txt");
            string suffixesTwo = File.ReadAllText(".\\Files\\suffix2.txt");
            string suffixesThree = File.ReadAllText(".\\Files\\suffix3.txt");
            string data         = File.ReadAllText(".\\Files\\BBC_train_full.csv");
            string stopWords    = File.ReadAllText(".\\Files\\stopWords.txt");


            //remove the return character
            suffixesOne = suffixesOne.Replace("\r\n", "\n");
            string[] suffixListOne = suffixesOne.Split('\n');

            suffixesTwo = suffixesTwo.Replace("\r\n", "\n");
            string[] suffixListTwo = suffixesTwo.Split('\n');

            suffixesThree = suffixesThree.Replace("\r\n", "\n");
            string[] suffixListThree = suffixesThree.Split('\n');

            //Create a suffix dictionary
            foreach (string suffixRule in suffixListOne)
            {
                string[] suffixPair = suffixRule.Split(',');
                Stemmer.suffixDictOne.Add(suffixPair[0], suffixPair[1]);
            }
            foreach (string suffixRule in suffixListTwo)
            {
                string[] suffixPair = suffixRule.Split(',');
                Stemmer.suffixDictTwo.Add(suffixPair[0], suffixPair[1]);
            }
            foreach (string suffixRule in suffixListThree)
            {
                string[] suffixPair = suffixRule.Split(',');
                Stemmer.suffixDictThree.Add(suffixPair[0], suffixPair[1]);
            }


            stopWords = stopWords.Replace("\r\n", "\n");
            string[] stopWordList = stopWords.Split('\n');

            //rectify Data
            data = data.Replace('.', ' ');
            data = data.Replace('-', ' ');
            data = new string((from c in data where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());
            
            //Parses Data into a 2-dimensional array of strings. the news stories are still one long string that needs to be parsed
            CSVData = parseCSVData(data);

            //Parse the long new stories into individual words
            List<List<string>> parsedWords = parseWords(CSVData);
             stemmedWords = new List<List<string>>(new List<string>[parsedWords.Count()]);
            for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
                stemmedWords[newsStory] = Stemmer.stemArray(parsedWords[newsStory]);

             lemmentedWords = new List<List<string>>(new List<string>[parsedWords.Count()]);
            for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
                lemmentedWords[newsStory] = Lemmenter.lemmentArray(parsedWords[newsStory]);


            //remove empty elements
            parsedWords[1].RemoveAll(element => element == "");
            //remove stop words
            parsedWords[1].RemoveAll(element => stopWordList.Contains(element));


        }

        public static List<List<string>> parseCSVData(string data)
        {
            //Split the CSV file
            string[] rows = data.Split('\n');
            string[] Categories = rows[0].Split(',');

            //initialize 2D array
            List<List<string>> returnData = new List<List<string>>(new List<string>[Categories.Length]);
            for (int i = 0;i < Categories.Length;i++) returnData[i] = new List<string>(new string[rows.Length]);
            
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
            //Initialize list of lists of words
            List<List<string>> parsedWords = new List<List<string>>(new List<string>[CSVData[0].Count()]);

            //for each new story, parse the words into a list of words
            for(int row = 0; row < CSVData[1].Count(); row++)
                parsedWords[row] = CSVData[1][row].Split(' ').ToList();

            return parsedWords;
        }
    }

}
