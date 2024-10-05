using LemmaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{

    public static class Lemmenter
    {
        public static Lemmatizer lemmatizer = new Lemmatizer(File.OpenRead(".\\Files\\full7z-multext-en.lem"));
        public static List<string> lemmentArray(List<string> words)
        {
            List<string> returnList = new List<string>();
            foreach (string word in words)
            {
                returnList.Add(lemmatizer.Lemmatize(word));
            }
            return returnList;
        }

        public static string lemmentWord(string word)
        {
            return lemmatizer.Lemmatize(word);
        }
    }
}
