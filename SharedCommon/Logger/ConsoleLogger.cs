namespace SharedCommon.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using SharedCommon.Extensions;

    public class ConsoleLogger : ILogger
    {
        private const string TIME_SEPARATOR = " | ";

        private readonly ConsoleSpinner spinner = new ConsoleSpinner();

        public DateTime LastLogTime { get; private set; }

        public void Log(Exception e)
        {
            this.LastLogTime = DateTime.Now;
            this.AddSeparator();
            Console.WriteLine(this.GetTimeString() + " EXCEPTION :");
            this.LogMultiLineMessage(e.ToString());
            this.AddSeparator();
        }

        public void AddSeparator()
        {
            Console.WriteLine(new string('-', Console.WindowWidth - (Console.WindowWidth / 3)));
        }

        public void Log(string template, params object[] data)
        {
            this.LastLogTime = DateTime.Now;

            if (data.Length == 0 && (template.Contains("\n") || template.Contains("\r\n")))
            {
                this.LogMultiLineMessage(template);
            }
            else
            {
                Console.Write(this.GetTimeString() + template + "\r\n", data);
            }
        }

        private void LogMultiLineMessage(string message)
        {
            string lineSeparator = message.Contains("\r\n") ? "\r\n" : "\n";
            IList<string> lines = message.Split(new[] { lineSeparator }, StringSplitOptions.None).ToList();
            lines.ForEach(l => Console.WriteLine(this.GetTimeString() + l));
        }

        public void LogToStatusLine(string template, params object[] data)
        {
            this.LastLogTime = DateTime.Now;

            int curLeft = Console.CursorLeft;
            int curTop = Console.CursorTop;

            Console.SetCursorPosition(Console.WindowLeft, Console.WindowHeight);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(Console.WindowLeft, Console.WindowHeight);

            this.spinner.Turn();

            Console.Write(" " + template, data);

            Console.SetCursorPosition(curLeft, curTop);
        }

        private string GetTimeString()
        {
            return this.LastLogTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + TIME_SEPARATOR;
        }
    }

    public class ConsoleSpinner
    {
        private int counter;

        public void Turn()
        {
            this.counter++;
            switch (this.counter % 4)
            {
                case 0:
                    Console.Write("/");
                    this.counter = 0;
                    break;
                case 1:
                    Console.Write("-");
                    break;
                case 2:
                    Console.Write("\\");
                    break;
                case 3:
                    Console.Write("|");
                    break;
            }
        }
    }
}