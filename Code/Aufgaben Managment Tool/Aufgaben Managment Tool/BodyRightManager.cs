namespace Aufgaben_Managment_Tool
{
    internal static class BodyRightManager
    {
        private static string _content = string.Empty;
        private static IRenderable? _renderable = null;
        private static string _title = "Information";
        private static readonly object _lock = new object();

        public static void SetTitle(string title)
        {
            lock (_lock)
            {
                _title = string.IsNullOrWhiteSpace(title) ? "Information" : title;
            }
        }

        public static void Set(string text)
        {
            lock (_lock)
            {
                _content = text ?? string.Empty;
                _renderable = null;
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
                _renderable = null;
            }
        }

        public static void SetRenderable(IRenderable renderable)
        {
            lock (_lock)
            {
                _renderable = renderable;
                _content = string.Empty;
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _content = string.Empty;
                _renderable = null;
            }
        }

        public static Panel GetPanel()
        {
            IRenderable content;
            string title;
            lock (_lock)
            {
                title = string.IsNullOrWhiteSpace(_title) ? "Information" : _title;

                if (_renderable != null)
                {
                    content = _renderable;
                }
                else
                {
                    var display = string.IsNullOrWhiteSpace(_content)
                        ? "[grey]Keine Informationen verfügbar[/]"
                        : _content;
                    content = new Markup(display);
                }
            }

            return new Panel(content)
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[yellow]{title}[/]"),
                Padding = new Padding(0, 0),
                Expand = true
            };
        }
    }
}
