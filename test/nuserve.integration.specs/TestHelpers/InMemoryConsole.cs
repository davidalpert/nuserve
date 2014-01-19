using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NuGet;
using NuGet.Common;

namespace nuserve.integration.specs.TestHelpers
{
    public class InMemoryConsole : IConsole
    {
        public InMemoryConsole()
        {
            Messages = new List<string>();
            Error = new StringWriter(new StringBuilder());
        }

        public List<string> Messages { get; private set; }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            Write(level + string.Format(message, args));
            WriteLine();
        }

        public void Write(object value)
        {
            if (value != null)
            {
                Write(value.ToString());
            }
        }

        public void Write(string value)
        {
            var count = Messages.Count;

            if (count == 0)
            {
                Messages.Add(string.Empty);
            }

            Messages[count - 1] += value;
        }

        public void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public void WriteLine()
        {
            Messages.Add(string.Empty);
        }

        public void WriteLine(object value)
        {
            Write(value);
            WriteLine();
        }

        public void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        public void WriteLine(string format, params object[] args)
        {
            Write(string.Format(format, args));
            WriteLine();
        }

        public void WriteError(object value)
        {
            Log(MessageLevel.Error, (value ?? string.Empty).ToString());
        }

        public void WriteError(string value)
        {
            Log(MessageLevel.Error, value);
        }

        public void WriteError(string format, params object[] args)
        {
            Log(MessageLevel.Error, format, args);
        }

        public void WriteWarning(string format)
        {
            Log(MessageLevel.Warning, format);
        }

        public void WriteWarning(string format, params object[] args)
        {
            Log(MessageLevel.Warning, format, args);
        }

        public bool Confirm(string description)
        {
            return (ConfirmDelegate ?? (s => true))(description);
        }

        public Func<string, bool> ConfirmDelegate;

        public void PrintJustified(int startIndex, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var targetWidth = startIndex + text.Length;
             
            WriteLine(text.PadLeft(targetWidth));
        }

        public void PrintJustified(int startIndex, string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return;

            var targetWidth = Math.Min(startIndex + text.Length, maxWidth);

            WriteLine(text.PadLeft(targetWidth));
        }

        public int CursorLeft { get; set; }
        public TextWriter Error { get; private set; }
        public int WindowWidth { get; set; }
    }
}