using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;
using System.IO;
using System.Text;


namespace RewindGame.Game.ExternalUtills
{
    class ProgressStore
    {
        public class ProgressData
        {
            public string Level;
            //Add properties here
        }
        static public string path  = "Progress.json";
        
        public static string readSavedLevel()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var data = JsonConvert.DeserializeObject<ProgressData>(sr.ReadToEnd());
                return data.Level;
            }
        }
        public static void StoreLevel(ProgressData data)
        {
            File.Create(path).Close();
            using (StreamWriter sw = new StreamWriter(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(sw, data);
            }
        }

        public static void Init()
        {
            using (StreamWriter w = File.AppendText(path));
        }
        public static bool DoesFileContainLevel()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var data = JsonConvert.DeserializeObject<ProgressData>(sr.ReadToEnd());
                if (data == null || data.Level == "")
                    return false;
                else
                    return true;
            }
        }
    }
}
