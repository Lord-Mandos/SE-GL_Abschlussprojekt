using Spectre.Console;
using System;

namespace Aufgaben_Managment_Tool
{
    internal static class BodyRightManager
    {
        private static string _content = string.Empty;
        private static readonly object _lock = new object();

        public static void Set(string text)
        {
            lock (_lock)
            {
                _content = text ?? string.Empty;
            }
        }

        public static void Add(string text)
        {
            if (text is null) return;
            lock (_lock)
            {
                if (string.IsNullOrEmpty(_content))
                    _content = text;
                else
                    _content += Environment.NewLine + text;
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _content = string.Empty;
            }
        }

        public static Panel GetPanel()
        {
            string display;
            lock (_lock)
            {
                display = string.IsNullOrWhiteSpace(_content)
                    ? "[grey]Keine Informationen verfügbar[/]"
                    : _content;
            }

            return new Panel(new Panel(new Markup(display))
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("[yellow]Information[/]")
            });
        }
    }
}
