﻿using LemmaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace Master
{
    public static class NaiveBayes
    {
        public static Dictionary<string, float[]> probabilityTable = new Dictionary<string, float[]>();
        public static float[] defaultProbability;
        public static Dictionary<string, float[]> wordCount = new Dictionary<string, float[]>();
        public static float vocabularySize;
        public static List<float> categoryTotalWordCount;
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
            for (int i = 0; i < categoryTotalWordCount.Count; i++) categoryTotalWordCount[i] = 0;
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
                    if (!probabilityTable.ContainsKey(word)) probabilityTable.Add(word, new float[categoryOrder.Count()]);
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

            foreach (string word in data)
            {
                float normAffordance = -1000;
                foreach (int index in categoryOrder.Values)
                {
                    //if the word has a probability
                    if (probabilityTable.ContainsKey(word))
                        probability[index] *= probabilityTable[word][index];
                    //else use the default probability
                    else
                        probability[index] *= defaultProbability[index];
                    float thisAffordance = (float)Math.Log10(probability[index]);
                    if (normAffordance < thisAffordance)
                        normAffordance = thisAffordance;
                }
                float offset = (float)Math.Pow(10, Math.Abs(Math.Floor(normAffordance)) + 1);
                //normalize the probabilities
                foreach (int index in categoryOrder.Values)
                {
                    probability[index] *= offset;
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
            foreach(string category in categoryOrder.Keys)
            {
                Console.WriteLine($"{category} : {probability[categoryOrder[category]]}");
            }


            return estimatedCategory;
        }
    }
}