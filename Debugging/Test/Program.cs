using Debugging;
using System;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            bool errorsFlag;
            int i;
            int lineCount = 10000;
            DirectoryInfo dir = new DirectoryInfo(Log.logPath);

            if (Directory.Exists(Log.logPath)) 
                dir.Delete(true);

            for (i = 0; i < lineCount; i++) Log.Write(i.ToString());

            Log.GoingBehindThread.WaitOne();

            i = 0;
            errorsFlag = false;
            foreach (string filePath in Directory.GetFiles(Log.logPath))
            {
                string line;
                using (var file = new System.IO.StreamReader(filePath))
                {
                    while ((line = file.ReadLine()) != null && errorsFlag == false)
                    {
                        if (i != int.Parse(line.Split(new char[] { ']' })[1]))
                        {
                            errorsFlag = true;
                            Console.WriteLine("Ошибка в последовательной записи логов. i = " + i);
                            break;
                        }
                        else
                            i++;
                    }
                }
            }

            if (errorsFlag == false) Console.WriteLine("Тест пройден.");

            Console.ReadLine();
        }
    }
}
