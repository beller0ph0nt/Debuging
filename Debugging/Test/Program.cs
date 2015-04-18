using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debugging;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            int lineCount = 10000;

            for (i = 0; i < lineCount; i++) Log.Write(i.ToString());

            Log.GoingBehindThread.WaitOne();

            i = 0;
            foreach (string filePath in Directory.EnumerateFiles(Log.logPath))
            {
                string line;
                using (var file = new System.IO.StreamReader(filePath))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (i != int.Parse(line.Split(new char[] { ']' })[1]))
                        {
                            Console.WriteLine("Ошибка в последовательной записи логов. i = " + i);
                            break;
                        }
                        else
                            i++;
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
