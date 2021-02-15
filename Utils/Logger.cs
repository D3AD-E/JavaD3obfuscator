using System;

namespace JarD3obfuscator.Utils
{
    static class Logger
    {
        const ConsoleColor COLOR_ERR = ConsoleColor.Red;
        const ConsoleColor COLOR_WARNING = ConsoleColor.Yellow;
        const ConsoleColor COLOR_INFO = ConsoleColor.DarkGray;
        const ConsoleColor COLOR_DEFAULT = ConsoleColor.Gray;

        const string MSG_ERR = "[ERROR] ";
        const string MSG_WARNING = "[WARNING] ";
        const string MSG_INFO = "[INFO] ";
        const string MSG_DEFAULT = "[MESSAGE] ";

        public static void Log(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_DEFAULT;

            Console.Write(MSG_DEFAULT);
            Console.WriteLine(message);

            Console.ForegroundColor = prevColor;
        }

        public static void LogWarning(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_WARNING;

            Console.Write(MSG_WARNING);
            Console.WriteLine(message);

            Console.ForegroundColor = prevColor;
        }

        public static void LogError(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_ERR;

            Console.Write(MSG_ERR);
            Console.WriteLine(message);

            Console.ForegroundColor = prevColor;
        }

        public static void LogInfo(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_INFO;

            Console.Write(MSG_INFO);
            Console.WriteLine(message);

            Console.ForegroundColor = prevColor;
        }
    }
}
