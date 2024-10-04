using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using LemmaSharp;
using Master;

namespace Main
{
    public static class Master
    {

        public static string[] stopWordList;

        public static List<List<string>> CSVData;
        public static List<List<string>> stemmedData;
        public static List<List<string>> lemmentedData;
        public static List<string> answers;

        public static void Main(string[] args)
        {
            List<string> vocabulary = new List<string>();
            stemLemment();
            foreach (List<string> newsStory in lemmentedData) {
                foreach (string word in newsStory) {
                    if (!vocabulary.Contains(word))
                    {
                        vocabulary.Add(word);
                        Console.Write(vocabulary.Count());
                        Console.SetCursorPosition(0, Console.CursorTop);
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Finished populating vocabulary");
            answers = CSVData[0];
            answers.RemoveAt(0);
            NaiveBayes.learnData(CSVData[0].GetRange(0,200), stemmedData.GetRange(0,200));
            Console.WriteLine("Finished learning data");
            string input = "";
            while(true)
            {
                input = Console.ReadLine();
                if (input == "end") break;
                int newsStory;
                try
                {
                    
                    newsStory = Convert.ToInt32(input);
                    Console.WriteLine(CSVData[1][newsStory]);
                    Console.WriteLine();
                    Console.ReadKey();
                    Console.Write("\b");
                    Console.WriteLine(NaiveBayes.estimateData(lemmentedData[Convert.ToInt32(input)]));
                    Console.WriteLine($"Correct Answer: {CSVData[0][newsStory]}");
                }
                catch (Exception e) {
                    try
                    {
                        List<string> parsedInput = lineToParsedWords(input);

                        Console.WriteLine(NaiveBayes.estimateData(Lemmenter.lemmentArray(parsedInput)));    
                        
                    }
                    catch {
                        Console.WriteLine("Your input is invalid");
                    }
                
                }

                
            }
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
            stopWordList = stopWords.Split('\n');

            //rectify Data
            data = data.Replace('.', ' ');
            data = data.Replace('-', ' ');
            data = new string((from c in data where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());
            
            //Parses Data into a 2-dimensional array of strings. the news stories are still one long string that needs to be parsed
            CSVData = parseCSVData(data);

            //Remove the CSV category data
            CSVData[0].RemoveAt(0);
            CSVData[1].RemoveAt(0);
            //Parse the single string news stories into individual words of multiple strings
            List<List<string>> parsedWords = parseCollection(CSVData);

            //remove empty elements
            parsedWords[1].RemoveAll(element => element == "");
            //remove stop words
            parsedWords[1].RemoveAll(element => stopWordList.Contains(element));

            //Stem the words
            stemmedData = new List<List<string>>(new List<string>[parsedWords.Count()]);
            for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
            {
                stemmedData[newsStory] = Stemmer.stemArray(parsedWords[newsStory]);
                Console.Write($"{newsStory + 1} / {parsedWords.Count()}");
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            Console.WriteLine();
            Console.WriteLine("Finished stemming");
            //lemment the words
             lemmentedData = new List<List<string>>(new List<string>[parsedWords.Count()]);
            for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
            {
                lemmentedData[newsStory] = Lemmenter.lemmentArray(parsedWords[newsStory]);
                Console.Write($"{newsStory + 1} / {parsedWords.Count()}");
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            Console.WriteLine();
            Console.WriteLine("Finished Lemmenting");



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

        public static List<List<string>> parseCollection(List<List<string>> CSVData)
        {
            //Initialize list of lists of words
            List<List<string>> parsedWords = new List<List<string>>(new List<string>[CSVData[0].Count()]);

            //for each new story, parse the words into a list of words
            for(int row = 0; row < CSVData[1].Count(); row++)
                parsedWords[row] = CSVData[1][row].Split(' ').ToList();

            return parsedWords;
        }

        public static List<string> lineToParsedWords(string words) {

            //rectify Data
            words = words.Replace('.', ' ');
            words = words.Replace('-', ' ');
            words = new string((from c in words where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());

            List<string> parsedWords = words.Split(' ').ToList();

            //remove empty elements
            parsedWords.RemoveAll(element => element == "");
            //remove stop words
            parsedWords.RemoveAll(element => stopWordList.Contains(element));

            return parsedWords;
        
        }
    }

}
