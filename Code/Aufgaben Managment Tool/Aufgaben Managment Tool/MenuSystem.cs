using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class MenuSystem
    {
        public static Markup mainMenuText = new Markup
        (
            "\n" +
            "[green]>[/] 1. Aufgabenverwaltung\n" +
            "[green]>[/] 2. Kanban-Board\n" +
            "[green]>[/] 3. Suchen\n" +
            "[green]>[/] 4. Abmelden\n" +
            "[green]>[/] 5. Beenden\n"
        );
        public static Panel MainMenu(Markup menuText, string menuTitle)
        {
            var panel = new Panel(
                new Panel(Align.Left(menuText, VerticalAlignment.Middle))
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[yellow]{menuTitle}[/]"),
                })
            {
                Padding = new Padding(3, 1, 8, 0),
                Expand = true
            };

            return panel;
        }

        public static void UserMenuChoice()
        {
            int choice = AnsiConsole.Ask<int>("Bitte wählen Sie eine Option:");
        }
    }
}
