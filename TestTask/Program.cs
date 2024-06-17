using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            singleLetterStats = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            doubleLetterStats = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.ReadKey();
            
        }

        /// <summary>
        /// Ф-ция считывающая из файла все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="arg">путь до файла</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> SingleLetterStats(string arg)
        {
            IReadOnlyStream inputStream = GetInputStream(arg);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream);

            return singleLetterStats;
        }

        /// <summary>
        /// Ф-ция считывающая из файла все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="arg">путь до файла</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> DoubleLetterStats(string arg)
        {
            IReadOnlyStream inputStream = GetInputStream(arg);

            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream);

            return doubleLetterStats;
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            IList<LetterStats> letterStats = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (char.IsLetter(c))
                {
                    LetterStats newletter = new LetterStats();
                    newletter.Letter = c.ToString();
                    newletter.Count = 1;
                    FillLetterStats(letterStats, newletter);
                }
            }
            letterStats = letterStats.Cast<LetterStats>().OrderBy(x => x.Letter).ToList();
            stream.CloseFile();

            return letterStats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            IList<LetterStats> letterStats = new List<LetterStats>();
            LetterStats newletter = new LetterStats();

            newletter = ResetLetterStat(newletter);
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                
                string c = stream.ReadNextChar().ToString();

                if (newletter.Letter != null && newletter.Letter.Length >= 2)
                {
                    if (newletter.Letter[0] == newletter.Letter[1])
                    {
                        FillLetterStats(letterStats, newletter);
                    }
                    newletter = ResetLetterStat(newletter);
                }

                if (char.IsLetter(c[0]))
                {
                    if (newletter.Letter == null || newletter.Letter.Length < 2)
                    {
                        newletter.Letter += c.ToUpper();
                    }
                }
            }
            letterStats = letterStats.Cast<LetterStats>().OrderBy(x => x.Letter).ToList();
            stream.CloseFile();

            return letterStats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:
                    letters = (from l in letters 
                               where Dictionary.consonants.ToUpper().Contains(l.Letter[0].ToString()) 
                               select l).ToList();
                    break;
                case CharType.Vowel:
                    letters = (from l in letters
                               where Dictionary.vowels.Contains(l.Letter[0].ToString().ToLower())
                               select l).ToList();
                    break;
            }
            return letters;
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            foreach (LetterStats letter in letters)
            {
                Console.WriteLine(letter.Letter + " - " + letter.Count);
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }

        /// <summary>
        /// Метод обновляет и добавляет новые данные в коллекцию, содержащую статистику по буквам/парам.
        /// </summary>
        /// <param name="letterStats">Коллекция со статистикой</param>
        /// <param name="newletter">буква/пара</param>
        private static void FillLetterStats(IList<LetterStats> letterStats, LetterStats newletter)
        {
            if (letterStats.Any(x => x.Letter == newletter.Letter))
            {
                newletter = letterStats.Cast<LetterStats>().Where(x => x.Letter == newletter.Letter).First();
                int index = letterStats.IndexOf(newletter);
                letterStats[index] = IncStatistic(newletter);
            }
            else
            {
                letterStats.Add(newletter);
            }
        }

        /// <summary>
        /// Метод подготавливает переданную статистику о букве/паре к дальнейшей работе.
        /// </summary>
        private static LetterStats ResetLetterStat(LetterStats letterStat)
        {
            letterStat.Letter = "";
            letterStat.Count = 1;
            return letterStat;
        }

        private static void testOutput(IEnumerable<LetterStats> letters)
        {
            foreach (LetterStats letter in letters)
            {
                Console.Out.WriteLine(letter.Letter + " - " + letter.Count);
            }
        }

    }
}
