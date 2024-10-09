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
        string returnWord = lemmatizer.Lemmatize(word);
        returnList.Add(returnWord);
      }
      return returnList;
    }

    public static string LemmatizeWord(string word)
    {
      Console.Write(word);
      Console.SetCursorPosition(20, Console.CursorTop);
      string returnWord = lemmatizer.Lemmatize(word);
      Console.WriteLine(returnWord);
      return returnWord;
    }
  }
}
