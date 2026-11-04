using System;
using System.Text;
using System.Threading;

namespace OsintHq.Engine
{
    public struct FormattedSpan
    {
        public string Text;
        public ConsoleColor Color;
        public FormattedSpan(string text, ConsoleColor color)
        {
            Text = text ?? "";
            Color = color;
        }
    }

    public static class AnsiConsole
    {
        private static readonly object _lock = new object();

        static AnsiConsole()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            catch { }
        }

        public static void PrintBoxedRow(int innerWidth, params FormattedSpan[] spans)
        {
            lock (_lock)
            {
                int visibleLength = 0;
                foreach (var s in spans)
                {
                    if (s.Text != null) visibleLength += s.Text.Length;
                }

                // If text exceeds width, truncate safely
                int available = innerWidth - 2;
                Write("  ║ ", ConsoleColor.DarkGray);
                
                int currentLen = 0;
                foreach (var s in spans)
                {
                    if (s.Text == null) continue;
                    if (currentLen + s.Text.Length <= available)
                    {
                        Write(s.Text, s.Color);
                        currentLen += s.Text.Length;
                    }
                    else
                    {
                        int allowed = available - currentLen;
                        if (allowed > 0)
                        {
                            Write(s.Text.Substring(0, allowed), s.Color);
                            currentLen += allowed;
                        }
                        break;
                    }
                }

                int remaining = innerWidth - 2 - currentLen;
                if (remaining > 0)
                {
                    Console.Write(new string(' ', remaining));
                }
                WriteLine(" ║", ConsoleColor.DarkGray);
            }
        }

        public static void Write(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            lock (_lock)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ForegroundColor = prev;
            }
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            lock (_lock)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = prev;
            }
        }

        public static void Typewriter(string text, ConsoleColor color, int delayMs = 12)
        {
            lock (_lock)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = color;
                foreach (char c in text)
                {
                    Console.Write(c);
                    if (delayMs > 0) Thread.Sleep(delayMs);
                }
                Console.WriteLine();
                Console.ForegroundColor = prev;
            }
        }

        public static void Header(string title, ConsoleColor color = ConsoleColor.Cyan)
        {
            WriteLine(new string('=', 78), color);
            WriteLine("  " + title.ToUpper(), ConsoleColor.White);
            WriteLine(new string('=', 78), color);
        }

        public static void SubHeader(string title, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            WriteLine("--- [ " + title + " ] " + new string('-', Math.Max(0, 70 - title.Length)), color);
        }

        public static void AgentTag(string agentName, ConsoleColor color, string message)
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[");
                Console.ForegroundColor = color;
                Console.Write(agentName);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void ProgressBar(string label, int steps = 20, int speedMs = 30)
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(label.PadRight(28) + " [");
                for (int i = 0; i <= steps; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("=");
                    Thread.Sleep(speedMs);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("] 100%");
                Console.ResetColor();
            }
        }

        public static void KeyValue(string key, string value, ConsoleColor keyColor = ConsoleColor.DarkGray, ConsoleColor valColor = ConsoleColor.White)
        {
            lock (_lock)
            {
                Console.ForegroundColor = keyColor;
                Console.Write("  {0,-22} : ", key);
                Console.ForegroundColor = valColor;
                Console.WriteLine(value);
                Console.ResetColor();
            }
        }

        public static void Status(string key, string status, bool isOk = true)
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  {0,-22} : ", key);
                if (isOk)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[ OK ] " + status);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ ALERT ] " + status);
                }
                Console.ResetColor();
            }
        }

        public static string Prompt(string promptText, ConsoleColor color = ConsoleColor.Yellow)
        {
            Write(promptText + " ", color);
            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine();
            Console.ResetColor();
            return input != null ? input.Trim() : string.Empty;
        }

        public static void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch { }
        }

        public static void PressAnyKey(string message = "Press any key to return...")
        {
            WriteLine("\n" + message, ConsoleColor.DarkGray);
            try
            {
                if (Console.IsInputRedirected) return;
                Console.ReadKey(true);
            }
            catch { }
        }
    }
}
