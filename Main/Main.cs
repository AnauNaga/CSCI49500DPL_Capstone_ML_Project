using System;
using System.IO;
using System.Collections.Generic;

namespace Main
{
    public static class Master
    {
        public static void Main(string[] args)
        {
            string affixes = File.ReadAllText("C:\\Users\\Anau Naga\\source\\repos\\CSCI49500DPL_Capstone_ML_Project\\Files\\prefix-suffix.txt");
            string data = File.ReadAllText("C:\\Users\\Anau Naga\\source\\repos\\CSCI49500DPL_Capstone_ML_Project\\Files\\BBC_train_full.csv");
            Console.Write(data);
            Console.ReadLine();

        }

        public static string[][] parseDataCSV(string data)
        {
            while (true)
            {
                
            }




            return null;
        }

    }

    
    public static class Stemming
    {
        public static void stemming()
        {

        }




    }

    public static class Lementization
    {
        public static void lemmentization()
        {

        }
    }

}
