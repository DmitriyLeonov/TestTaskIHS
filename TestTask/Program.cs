using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using TestTask;

namespace ConsoleApplication5
{
    class Program
    {
        
        static void Main(string[] args)
        {
            bool fileExist = false;
            string path = Console.ReadLine();
            while (fileExist == false)
            {
                if (File.Exists(path))
                {
                    fileExist = true;
                }
                else if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    Console.WriteLine("Can't read file, try again");
                    path = Console.ReadLine();
                }
                else
                {
                    fileExist = true;
                }
            }
            List<Population> populations = new List<Population>();
            var GetDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Thread thread1 = new Thread(new ParameterizedThreadStart(ReadFromLocalFile));
            Thread thread2 = new Thread(new ParameterizedThreadStart(ReadFromURL));
            thread1.Start(path);
            thread2.Start(path);
            Thread.Sleep(10000);
            StreamReader reader = new StreamReader(GetDirectory + @"\DowloadedFile.txt");
            string line;
            string letters = "";
            string numbers = "";
            while ((line = reader.ReadLine()) != null)
            {
                foreach (char ch in line)
                {
                 if (char.IsDigit(ch))
                      numbers += ch;
                if (char.IsLetter(ch))
                      letters += ch;
                }
                populations.Add(new Population
                {
                    City = letters,
                    CityPopulation = Int32.Parse(numbers)
                });
                letters = "";
                numbers = "";
            }
            var result = populations.GroupBy(x => x.City)
                    .Select(x => new Population
                    { City = x.Key, CityPopulation = x.Sum(v => v.CityPopulation) });
            foreach (var r in result)
            {
                Console.WriteLine(string.Format("{0} {1}", r.City, r.CityPopulation));
            }
            Console.ReadKey();
        }
        
        public static void ReadFromLocalFile(object path)
        {
            try
            {
                var GetDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                StreamReader sr = new StreamReader((string)path);
                string str = sr.ReadToEnd();
                FileStream file = new FileStream(GetDirectory + @"\DowloadedFile.txt", FileMode.OpenOrCreate);
                byte[] array = System.Text.Encoding.Default.GetBytes(str);
                file.Write(array, 0, array.Length);
                file.Close();
            }
            catch
            {
                
            }
        }

        public static void ReadFromURL(object path)
        {
            try
            {
                var GetDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                WebClient client = new WebClient();
                File.Delete(GetDirectory + @"\DowloadedFile.txt");
                Uri uri = new Uri((string)path);
                client.DownloadFile(uri, GetDirectory + @"\DowloadedFile.txt");
            }
            catch
            {

            }
        }
    }
}