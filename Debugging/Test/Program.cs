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
            DirectoryInfo dir = new DirectoryInfo(Log.LogPath);

            if (Directory.Exists(Log.LogPath))
                dir.Delete(true);

            Console.WriteLine("Записываем данные в файл...");

            // записываем в файл последовательность цифр
            for (i = 0; i < lineCount; i++)
            {
                Log.Write(i.ToString());

                if (i % 100 == 0)
                    Console.Write(".");
            }

            Console.WriteLine("\nОжидаем завершения записи данных в файл...");

            // ждем, пока завершиться запись в лог
            Log.WaitWriteFinish();

            Console.WriteLine("Запись завершена");
            Console.WriteLine("Осуществляем проверку записанных данных");

            // осуществляем проверку записанной последовательности
            i = 0;
            errorsFlag = false;
            foreach (string filePath in Directory.GetFiles(Log.LogPath))
            {
                string line;
                using (var file = new System.IO.StreamReader(filePath))
                {
                    while ((line = file.ReadLine()) != null && errorsFlag == false)
                    {
                        // если считанный номер не совпал с текущим
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

            if (errorsFlag == false)
                Console.WriteLine("Тест пройден успешно.");

            Console.ReadLine();
        }
    }
}
