using System;
using System.Collections.Generic;
using System.Threading;
using OsintHq.Engine;

namespace OsintHq.Agents
{
    public abstract class AgentBase
    {
        public string Name { get; set; }
        public string Codename { get; set; }
        public string Role { get; set; }
        public ConsoleColor Color { get; set; }
        public string OfficeName { get; set; }
        public string OfficeLayout { get; set; }
        public string OfficeAscii { get; set; }
        public string Appearance { get; set; }
        public string Behavior { get; set; }
        public string CollaborationStyle { get; set; }
        public string Status { get; set; }
        public List<string> Equipment { get; set; }

        protected AgentBase()
        {
            Equipment = new List<string>();
            Status = "STANDBY // ALL SYSTEMS NOMINAL";
        }

        public void Speak(string message)
        {
            AnsiConsole.AgentTag(Codename + " (" + Name + ")", Color, message);
        }

        public void Think(string thought)
        {
            AnsiConsole.AgentTag(Codename + " // INTERNAL", ConsoleColor.DarkGray, "..." + thought + "...");
        }

        public void DisplayOffice()
        {
            AnsiConsole.SafeClear();
            AnsiConsole.Header("VIRTUAL WORKSPACE // " + OfficeName.ToUpper(), Color);
            AnsiConsole.WriteLine("AGENT ASSIGNED : " + Name + " [Codename: " + Codename + "]", ConsoleColor.White);
            AnsiConsole.WriteLine("OPERATIONAL ROLE: " + Role, ConsoleColor.Gray);
            AnsiConsole.WriteLine("CURRENT STATUS  : " + Status, ConsoleColor.Green);
            Console.WriteLine();

            // Print the ASCII art of the physical office/stand
            AnsiConsole.WriteLine(OfficeAscii, Color);
            Console.WriteLine();

            AnsiConsole.SubHeader("STATION ARCHITECTURE & PHYSICAL WORKSPACE");
            AnsiConsole.WriteLine(OfficeLayout, ConsoleColor.DarkYellow);
            Console.WriteLine();

            AnsiConsole.SubHeader("OPERATOR DOSSIER & MANNERISMS");
            AnsiConsole.KeyValue("Physical Form", Appearance, ConsoleColor.DarkCyan, ConsoleColor.Gray);
            AnsiConsole.KeyValue("Behavior Pattern", Behavior, ConsoleColor.DarkCyan, ConsoleColor.Gray);
            AnsiConsole.KeyValue("Collaboration Mode", CollaborationStyle, ConsoleColor.DarkCyan, ConsoleColor.Gray);
            Console.WriteLine();

            AnsiConsole.SubHeader("DEPLOYED HARDWARE & STAND INSTRUMENTS");
            foreach (var item in Equipment)
            {
                AnsiConsole.WriteLine("  [*] " + item, ConsoleColor.Cyan);
            }
            Console.WriteLine();
        }

        public abstract string RespondToInquiry(string query);
    }
}
