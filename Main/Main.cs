using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using LemmaSharp;
using Master;
using System.Net;

namespace Main
{
  public static class Master
  {

    public static string[] stopWordList;

    public static List<List<string>> CSVData;
    public static List<List<string>> CSVDataTest;
    public static List<List<string>> stemmedData;
    public static List<List<string>> stemmedDataTest;
    public static List<List<string>> lemmentedData;
    public static List<List<string>> lemmentedDataTest;
    public static List<List<string>> stemlemmaData;
    public static List<string> categories;
    public static List<string> answers;
    public static List<string> vocabulary;
    public static Dictionary<string, int> vocabCount;
    public static Dictionary<string, int[]> vocabCountPerCategory;
    public static Dictionary<int, int> groupCount;
    public static Dictionary<string, int> NumberOfArticlesContaingWord;


    public static readonly int wordMaxOccurrances = 800;
    public static readonly int wordMinOccurrances = 50;
    public static void Main(string[] args)
    {
      //stem and lemmatize vocabulary
      vocabulary = new List<string>();
      vocabCount = new Dictionary<string, int>();
      groupCount = new Dictionary<int, int>();
      vocabCountPerCategory = new Dictionary<string, int[]>();
      stemLemment();

      string training1 = File.ReadAllText(".\\Files\\Training1.txt");
      List<List<string>> TrainingCSV1 = parseCSVData(training1);
      string training2 = File.ReadAllText(".\\Files\\Training2.txt");
      List<List<string>> TrainingCSV2 = parseCSVData(training2);
      string training3 = File.ReadAllText(".\\Files\\Training3.txt");
      List<List<string>> TrainingCSV3 = parseCSVData(training3);

      //add the list of answers
      //Count the number of articles that have a given category
      answers = CSVData[0];
      categories = new List<string>();
      foreach (string answer in answers)
      {
        if (!categories.Contains(answer)) 
          categories.Add(answer); 
      }

      int newsLabel = 0;
      NumberOfArticlesContaingWord = new Dictionary<string, int>();
      foreach (List<string> newsStory in lemmentedData)
      {
        Dictionary<string, int[]> tempVocabCountPerCategory = new Dictionary<string, int[]>();
        foreach (string word in newsStory)
        {
          if (!vocabCount.ContainsKey(word))
            vocabCount.Add(word, 0);
          if (!tempVocabCountPerCategory.ContainsKey(word))
            tempVocabCountPerCategory.Add(word, new int[5]);
          vocabCount[word] += 1;
          tempVocabCountPerCategory[word][categories.IndexOf(CSVData[0][newsLabel])] += 1;
          if (!vocabulary.Contains(word))
          {
            vocabulary.Add(word);
            Console.Write(vocabulary.Count());
            Console.SetCursorPosition(0, Console.CursorTop);
          }
        }
        foreach (string word in tempVocabCountPerCategory.Keys) {
          if (!vocabCountPerCategory.ContainsKey(word))
            vocabCountPerCategory.Add(word, new int[5]);
          if (!NumberOfArticlesContaingWord.ContainsKey(word))
            NumberOfArticlesContaingWord.Add(word, 0);
          for (int i = 0; i < 5; i++)
            if (tempVocabCountPerCategory[word][i] > 0)
            {
              vocabCountPerCategory[word][i] += 1;
              NumberOfArticlesContaingWord[word] += 1;
            }

        }
        newsLabel++;
      }
      Console.WriteLine();
      vocabCount = vocabCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
      
      //vocabulary = vocabulary.Where(x => ((float)vocabCountPerCategory[x].Max()/ (float)NumberOfArticlesContaingWord[x]) >= 0.5 && NumberOfArticlesContaingWord[x] > 20).ToList();
      //for (int i = 0; i < vocabulary.Count(); i++)
      //{
      //  Console.WriteLine($"{vocabulary[i]} : {vocabCount[vocabulary[i]]} : {vocabCountPerCategory[vocabulary[i]][0]}, {vocabCountPerCategory[vocabulary[i]][1]}, {vocabCountPerCategory[vocabulary[i]][2]}, {vocabCountPerCategory[vocabulary[i]][3]}, {vocabCountPerCategory[vocabulary[i]][4]}");
      //}

       Console.WriteLine();
      Console.WriteLine($"Finished populating vocabulary : {vocabulary.Count()} words");

      NaiveBayes Agent1 = new NaiveBayes();
      NaiveBayes Agent2 = new NaiveBayes();
      NaiveBayes Agent3 = new NaiveBayes();


      List<List<string>> stemmedTrainingData1 = parseCollection(TrainingCSV1);
      Agent1.learnData(TrainingCSV1[0], stemmedTrainingData1);

      List<List<string>> stemmedTrainingData2 = parseCollection(TrainingCSV2);
      Agent2.learnData(TrainingCSV2[0], stemmedTrainingData2);

      List<List<string>> TrainingCSV4 = new List<List<string>>();
      TrainingCSV4.Add(new List<string>());
      TrainingCSV4.Add(new List<string>());
      TrainingCSV4[0].AddRange(TrainingCSV1[0]);
      TrainingCSV4[1].AddRange(TrainingCSV1[1]);
      TrainingCSV4[0].AddRange(TrainingCSV2[0]);
      TrainingCSV4[1].AddRange(TrainingCSV2[1]);
      List<List<string>> stemmedTrainingData4 = parseCollection(TrainingCSV4);
      //learn data - Naive Bayes
      Agent3.learnData(TrainingCSV4[0], stemmedTrainingData4);
      Console.WriteLine("Finished learning data");
      string input = "";

      //learn data - FFNN
      //FFNN ffnn = new FFNN();
      //ffnn.initFFNN(vocabulary.Count(), 100, 20, 5);
      //ffnn.estimateData(ConvertStoryToInput(lemmentedData[0]));
      //var consoleYpos = Console.CursorTop;

      //float[][] StoriesAsInput = convertListOfStoriesToInput(lemmentedData);
      //string[] LabelsAsArray = CSVData[0].ToArray();
      //for (int i = 0; i < 100; i++)
      //{
      //  Console.SetCursorPosition(0, consoleYpos);
      //  ffnn.learnSet(LabelsAsArray, StoriesAsInput, 50);
      //  Console.Write(i);
      //}
      //ffnn.estimateData(ConvertStoryToInput(lemmentedData[0]));

      Console.WriteLine();
      int correctCount = 0;
      int totalAttempts = 0;
      int[] correctCountPerCategory = new int[5];
      //for (int i = 0; i < CSVDataTest[0].Count; i++)
      //{
      //  int estimatedAnswer = ffnn.estimateData(ConvertStoryToInput(lemmentedDataTest[i]));
      //  //Console.WriteLine(estimatedAnswer);
      //  if (categories[estimatedAnswer] == CSVDataTest[0][i])
      //  {
      //    correctCount++;
      //    correctCountPerCategory[estimatedAnswer]++;
      //  }
      //  totalAttempts++;
      //  Console.SetCursorPosition(0, Console.CursorTop);
      //}
      //Console.WriteLine($"{correctCount}/{totalAttempts}");
      //for (int i = 0; i < categories.Count; i++)
      //  Console.WriteLine($"{categories[i]} : {correctCountPerCategory[i]}");
      //Console.ReadKey();
      //Console.Write('\b');
      //Test


      Console.WriteLine();
      correctCount = 0;
      totalAttempts = 0;
      correctCountPerCategory = new int[5];
      for (int i = 0; i < CSVDataTest[0].Count; i++)
      {
        string estimatedAnswer = Agent1.estimateData(lemmentedDataTest[i]);
        //Console.WriteLine(estimatedAnswer);
        if (estimatedAnswer == CSVDataTest[0][i])
        {
          Agent1.totalCorrect++;
          Agent1.totalCorrectPerCategory[categories.IndexOf(estimatedAnswer)]++;
        }
        Agent1.totalAttemptsPerCategory[categories.IndexOf(CSVDataTest[0][i])]++;
        Agent1.totalAttempts++;

        estimatedAnswer = Agent2.estimateData(lemmentedDataTest[i]);
        //Console.WriteLine(estimatedAnswer);
        if (estimatedAnswer == CSVDataTest[0][i])
        {
          Agent2.totalCorrect++;
          Agent2.totalCorrectPerCategory[categories.IndexOf(estimatedAnswer)]++;
        }
        Agent2.totalAttemptsPerCategory[categories.IndexOf(CSVDataTest[0][i])]++;
        Agent2.totalAttempts++;

        estimatedAnswer = Agent3.estimateData(lemmentedDataTest[i]);
        //Console.WriteLine(estimatedAnswer);
        if (estimatedAnswer == CSVDataTest[0][i])
        {
          Agent3.totalCorrect++;
          Agent3.totalCorrectPerCategory[categories.IndexOf(estimatedAnswer)]++;
        }
        Agent3.totalAttemptsPerCategory[categories.IndexOf(CSVDataTest[0][i])]++;
        Agent3.totalAttempts++;
        Console.SetCursorPosition(0, Console.CursorTop);
      }
      Console.WriteLine($"{Agent1.totalCorrect}/{Agent1.totalAttempts}");
      Console.WriteLine($"{"Category",-20}Correct");
      for (int i = 0; i < categories.Count(); i++)
        Console.WriteLine($"{categories[i],-20} :{Agent1.totalCorrectPerCategory[i]} / {Agent1.totalAttemptsPerCategory[i]}");

      Console.WriteLine($"{Agent2.totalCorrect}/{Agent2.totalAttempts}");
      Console.WriteLine($"{"Category",-20}Correct");
      for (int i = 0; i < categories.Count(); i++)
        Console.WriteLine($"{categories[i],-20} :{Agent2.totalCorrectPerCategory[i]} / {Agent2.totalAttemptsPerCategory[i]}");

      Console.WriteLine($"{Agent3.totalCorrect}/{Agent3.totalAttempts}");
      Console.WriteLine($"{"Category",-20}Correct");
      for (int i = 0; i < categories.Count(); i++)
        Console.WriteLine($"{categories[i],-20} :{Agent3.totalCorrectPerCategory[i]} / {Agent3.totalAttemptsPerCategory[i]}");

      Console.ReadKey();
      Console.Write('\b');
      int correct = 0;
      int newsStoryIndex = 0;
      while (true)
      {

        //input = Console.ReadLine();
        if (input == "end") break;
        try
        {

          //newsStoryIndex = Convert.ToInt32(input);
          //Console.WriteLine(CSVData[1][newsStory]);
          //Console.WriteLine();
          //Console.ReadKey();
          //Console.Write("\b");
          string outcome = Agent1.estimateData(lemmentedDataTest[Convert.ToInt32(input)]);
          //Console.WriteLine("Naive Bayes: " + outcome));
          //Console.WriteLine("FFNN       : "+FFNN.estimateData(lemmentedData[Convert.ToInt32(input)]));
          //Console.WriteLine($"Correct Answer: {CSVDataTest[0][newsStory]}");
          if(CSVDataTest[0][newsStoryIndex] == outcome)correct++;
        }
        catch (Exception e)
        {
          try
          {
            List<string> parsedInput = lineToParsedWords(input);

            Console.WriteLine(Agent1.estimateData(Lemmenter.lemmentArray(parsedInput)));

          }
          catch
          {
            Console.WriteLine("Your input is invalid");
          }

        }

        newsStoryIndex++;
        if (newsStoryIndex == CSVDataTest[0].Count) break;
      }
      Console.WriteLine($"{correct}/{CSVDataTest.Count}");
      Console.ReadLine();
    }

    public static void stemLemment()
    {
      //read given files
      string suffixesOne = File.ReadAllText(".\\Files\\suffix1.txt");
      string suffixesTwo = File.ReadAllText(".\\Files\\suffix2.txt");
      string suffixesThree = File.ReadAllText(".\\Files\\suffix3.txt");
      string data = File.ReadAllText(".\\Files\\BBC_train_full.csv");
      string testData = File.ReadAllText(".\\Files\\test_data(2).csv");
      string stopWords = File.ReadAllText(".\\Files\\stopWords.txt");

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

      //Parses Data into a 2-dimensional array of strings. the news stories are still one long string that needs to be parsed
      CSVData = parseCSVData(data);
      CSVDataTest = parseCSVData(testData);


      //Parse the single string news stories into individual words of multiple strings
      List<List<string>> parsedWords = parseCollection(CSVData);
      List<List<string>> parsedWordsTest = parseCollection(CSVDataTest);

      foreach (List<string> newsStory in parsedWords)
      {
        //remove all \r
        newsStory.RemoveAll(element => element == "\r");
        //remove empty elements
        newsStory.RemoveAll(element => element == "");
        //remove stop words
        newsStory.RemoveAll(element => stopWordList.Contains(element));
      }

      foreach (List<string> newsStory in parsedWordsTest)
      {
        //remove all \r
        newsStory.RemoveAll(element => element == "\r");
        //remove empty elements
        newsStory.RemoveAll(element => element == "");
        //remove stop words
        newsStory.RemoveAll(element => stopWordList.Contains(element));
      }


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

      //Stem the words
      stemmedDataTest = new List<List<string>>(new List<string>[parsedWordsTest.Count()]);
      for (int newsStory = 0; newsStory < parsedWordsTest.Count(); newsStory++)
      {
        stemmedDataTest[newsStory] = Stemmer.stemArray(parsedWordsTest[newsStory]);
        Console.Write($"{newsStory + 1} / {parsedWordsTest.Count()}");
        Console.SetCursorPosition(0, Console.CursorTop);
      }
      Console.WriteLine();
      Console.WriteLine("Finished stemming");

      //stem lemmatized the words
      stemlemmaData = new List<List<string>>(new List<string>[parsedWords.Count()]);
      for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
      {
        stemlemmaData[newsStory] = Lemmenter.lemmentArray(stemmedData[newsStory]);
        Console.Write($"{newsStory + 1} / {parsedWords.Count()}");
        Console.SetCursorPosition(0, Console.CursorTop);
      }
      Console.WriteLine();
      Console.WriteLine("Finished stem-Lemmenting");

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

      //lemment the words
      lemmentedDataTest = new List<List<string>>(new List<string>[parsedWordsTest.Count()]);
      for (int newsStory = 0; newsStory < parsedWordsTest.Count(); newsStory++)
      {
        lemmentedDataTest[newsStory] = Lemmenter.lemmentArray(parsedWordsTest[newsStory]);
        Console.Write($"{newsStory + 1} / {parsedWordsTest.Count()}");
        Console.SetCursorPosition(0, Console.CursorTop);
      }
      Console.WriteLine();
      Console.WriteLine("Finished Lemmenting");

      //File.Create(".\\Files\\Stem.txt");
      //File.Create(".\\Files\\Stem-Lemma.txt");
      //File.Create(".\\Files\\Lemma.txt");

      StreamWriter stem = new StreamWriter(".\\Files\\Stem.txt");
      StreamWriter stemLemma = new StreamWriter(".\\Files\\Stem-Lemma.txt");
      StreamWriter lemma = new StreamWriter(".\\Files\\Lemma.txt");

      for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
      {
        stem.Write(CSVData[0][newsStory] + ',');
        stemLemma.Write(CSVData[0][newsStory] + ',');
        lemma.Write(CSVData[0][newsStory] + ',');
        string stemLine = "";
        string stemlemmaLine = "";
        string lemmaLine = "";

        for (int word = 0; word < stemmedData[newsStory].Count(); word++)
        {

          stemLine += stemmedData[newsStory][word] + ' ';
          stemlemmaLine += stemlemmaData[newsStory][word] + ' ';
          lemmaLine += lemmentedData[newsStory][word] + ' ';
        }
        stemLine = stemLine.Remove(stemLine.Length - 1);
        stemlemmaLine = stemlemmaLine.Remove(stemlemmaLine.Length - 1);
        lemmaLine = lemmaLine.Remove(stemlemmaLine.Length-1);
        stem.Write(stemLine);
        stemLemma.Write(stemlemmaLine);
        lemma.Write(lemmaLine);
        stem.Write('\n');
        stemLemma.Write('\n');
        lemma.Write('\n');
      }

      for (int newsStory = 0; newsStory < parsedWords.Count(); newsStory++)
      {
        stem.Write(CSVData[0][newsStory] + ',');
        stemLemma.Write(CSVData[0][newsStory] + ',');
        lemma.Write(CSVData[0][newsStory] + ',');
        string stemLine = "";
        string stemlemmaLine = "";
        string lemmaLine = "";

        for (int word = 0; word < stemmedData[newsStory].Count(); word++)
        {

          stemLine += stemmedData[newsStory][word] + ' ';
          stemlemmaLine += stemlemmaData[newsStory][word] + ' ';
          lemmaLine += lemmentedData[newsStory][word] + ' ';
        }
        stemLine = stemLine.Remove(stemLine.Length - 1);
        stemlemmaLine = stemlemmaLine.Remove(stemlemmaLine.Length - 1);
        lemmaLine = lemmaLine.Remove(stemlemmaLine.Length - 1);
        stem.Write(stemLine);
        stemLemma.Write(stemlemmaLine);
        lemma.Write(lemmaLine);
        stem.Write('\n');
        stemLemma.Write('\n');
        lemma.Write('\n');
      }

    }

    public static List<List<string>> parseCSVData(string data)
    {

      data = data.Replace('.', ' ');
      data = data.Replace('-', ' ');
      data = new string((from c in data where char.IsWhiteSpace(c) || char.IsLetter(c) || (c == '\n') || (c == ',') select c).ToArray());


      //Split the CSV file
      string[] rows = data.Split('\n');
      string[] Categories = rows[0].Split(',');

      //initialize 2D array
      List<List<string>> returnData = new List<List<string>>(new List<string>[Categories.Length]);
      for (int i = 0; i < Categories.Length; i++) returnData[i] = new List<string>(new string[rows.Length]);

      for (int row = 0; row < rows.Length; row++)
      {
        string[] rowSplit = rows[row].Split(',');
        //Add News Category
        returnData[0][row] = rowSplit[0];
        //Add News Text
        returnData[1][row] = rowSplit[1];
      }

      //Remove the CSV category data
      foreach (var column in returnData)
        column.RemoveAt(0);
      return returnData;
    }

    public static List<List<string>> parseCollection(List<List<string>> CSVData)
    {
      //Initialize list of a lists of words
      List<List<string>> parsedWords = new List<List<string>>(new List<string>[CSVData[0].Count()]);

      //for each news story, parse the words into a list of words
      for (int row = 0; row < CSVData[1].Count(); row++)
        parsedWords[row] = CSVData[1][row].Split(' ').ToList();

      return parsedWords;
    }

    public static float[][] convertListOfStoriesToInput(List<List<string>> newsStoryList) {
      float[][] returnFloatArray = new float[newsStoryList.Count][];
      for (int storyIndex = 0; storyIndex < newsStoryList.Count();storyIndex++)
      {
        returnFloatArray[storyIndex] = ConvertStoryToInput(newsStoryList[storyIndex]);
      } 
      return returnFloatArray;
    }

    public static float[] ConvertStoryToInput(List<string> newsStory)
    {
      float[] returnFloatArray = new float[vocabulary.Count()];
      for (int i = 0; i < returnFloatArray.Length; i++) returnFloatArray[i] = 0;
      foreach (string s in newsStory) {
        int index = vocabulary.IndexOf(s);
        if (index != -1)
          returnFloatArray[index]++;
      
      }
      return returnFloatArray;
    }

    public static List<string> lineToParsedWords(string words)
    {

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
