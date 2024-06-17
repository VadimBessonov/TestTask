using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _localStreamReader; // необходим для правильного чтения кодировки UTF-8

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;
            _localStream = new FileStream(fileFullPath, FileMode.Open);
            _localStreamReader = new StreamReader(_localStream); // необходим для правильного чтения кодировки UTF-8
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }
        

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof == true)
            {
                throw new Exception("end of file");
            }
           
            //int nextChar = _localStream.ReadByte(); // работает только с английскими символами, либо ASCII кодировкой
            int nextChar = _localStreamReader.Read();

            switch (nextChar)
            {
                case -1:
                    IsEof = true;
                    return '\0';
                default:
                    return (char)nextChar;
            }
        }

        /// <summary>
        /// Метод закрытия файла.
        /// </summary>
        public void CloseFile()
        {
            _localStreamReader.Close();
            _localStream.Close();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }
    }
}
