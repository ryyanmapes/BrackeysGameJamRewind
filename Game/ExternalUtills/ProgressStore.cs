using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;


namespace RewindGame.Game.ExternalUtills
{
    class ProgressStore
    {
        public static string readSavedLevel()
        {
            string path = "Level.txt";


            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadLine();
            }
        }
        public static void StoreLevel(string level)
        {
            string path = "Level.txt";
            File.Create(path).Close();
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(level);
            }
        }

        public static void Init()
        {
            string path = "Level.txt";
            using (StreamWriter w = File.AppendText(path)) ;
        }
    }
}
