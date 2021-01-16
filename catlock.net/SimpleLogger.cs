using System;

namespace x11dotnet
{
    public class SimpleLogger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        public LogLevel Level;

        public SimpleLogger(LogLevel level)
        {
            this.Level = level;
        }

        private void Write(string message, LogLevel message_level)
        {
            if (message_level <= Level)
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt")} {message_level} {message}");

        }

        public void Debug(string message)
        {
            Write(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            Write(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            Write(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            Write(message, LogLevel.Error);
        }
    }
}
