using LemmaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    public static class NaiveBayes
    {
        public static Dictionary<string, float[]> probabilityTable = new Dictionary<string, float[]>();
        public static float[] defaultProbability;
        public static Dictionary<string, float[]> wordCount = new Dictionary<string, float[]>();
        public static float vocabularySize;
        public static List<float> categoryTotalWordCount = new List<float>();
        public static Dictionary<string, int> categoryCount = new Dictionary<string, int>();
        public static float trainingDataCount = 0;
        public static Dictionary<string,int> categoryOrder = new Dictionary<string, int>();


        public static void learnData(List<string> answers, List<List<string>> trainingData)
        {
            trainingDataCount = trainingData.Count;

            //Count the number of categories are in the training data
            int categoryIndex = 0;
            foreach(string answer in answers) 
                if(categoryCount.ContainsKey(answer)) categoryCount[answer]++;
                else {categoryCount.Add(answer, 1); categoryOrder.Add(answer,categoryIndex); categoryIndex++; }

            categoryTotalWordCount = new List<float>(new float[categoryOrder.Count()]);
            defaultProbability = new float[categoryOrder.Count()];

            //Count the words for each category
            //Add the word to the dictionary if it is missing
            for (int newsStory = 0;newsStory < trainingData.Count(); newsStory++)
            {
                
                foreach(string word in trainingData[newsStory])
                {
                    //if the word is missing from the dictionary, add it
                    if (!wordCount.ContainsKey(word))
                        wordCount.Add(word, new float[categoryIndex + 1]);
                    //increase the counter for the word
                    wordCount[word][categoryOrder[answers[newsStory]]]++;
                    //increase the total word counter for a category
                    categoryTotalWordCount[categoryOrder[answers[newsStory]]]++;
                }
            }

            vocabularySize = wordCount.Count();

            //calculate the probability for each word in each category
            foreach(string word in wordCount.Keys)
            {
                foreach(string category in categoryOrder.Keys)
                {
                    int index = categoryOrder[category];
                    probabilityTable[word][index] = (wordCount[word][index] + 1) / (categoryTotalWordCount[index]+vocabularySize);
                }
            }

            //calculate the default probability
            foreach (string category in categoryOrder.Keys)
            {
                int index = categoryOrder[category];
                defaultProbability[index] = 1 / (categoryTotalWordCount[index] + vocabularySize);
            }
        }

        public static string estimateData(List<string> data)
        {
            List<float> probability = new List<float>(new float[categoryOrder.Count()]);
            foreach (string index in categoryOrder.Keys)
            {
                probability[categoryOrder[index]] = categoryCount[index] / trainingDataCount;
            }

            foreach (string category in categoryOrder.Keys)
            {
                int index = categoryOrder[category];
                foreach (string word in data)
                {
                    if (probabilityTable.ContainsKey(word))
                        probability[index] *= probabilityTable[word][index];
                    else
                        probability[index] *= defaultProbability[index];
                }
            }

            //find the largest number
            float largestProb = 0;
            string estimatedCategory = "";
            foreach (string category in categoryOrder.Keys)
            {
                if (largestProb < probability[categoryOrder[category]])
                {
                    largestProb = probability[categoryOrder[category]];
                    estimatedCategory = category;
                }
            }
            return estimatedCategory;
        }
    }
}
