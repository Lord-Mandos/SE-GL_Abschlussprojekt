using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class MenuSystem
    {
        public static List<Markup> mainMenuText = new List<Markup>
        {
            new Markup("[green]>[/] 1. Aufgabenverwaltung"),
            new Markup("[green]>[/] 2. Kanban-Board"),
            new Markup("[green]>[/] 3. Suchen"),
            new Markup("[green]>[/] 4. Abmelden"),
            new Markup("[green]>[/] 5. Beenden")
        };
        public static Panel MenuPanel(List<Markup> menuText, string menuTitle)
        {
            var panel = new Panel(new Panel(Align.Left
                (new Rows(menuText), VerticalAlignment.Middle))
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

        public static void UserMenuChoice(List<Markup> menuText)
        {
            int choice = AnsiConsole.Ask<int>("Bitte wählen Sie eine Option:");
            if (menuText == mainMenuText)
            {
                switch (choice)
                {
                    case 1:
                        //UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        //UIRenderer.UIMain(kanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
                        //UIRenderer.UIMain(searchMenuText, "Suchen");
                        break;
                    case 4:
                        // Abmelden logic here                        
                        break;
                    case 5:
                        AnsiConsole.MarkupLine("[red]Programm wird beendet...[/]");
                        Environment.Exit(0);
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            //else if (menuText == taskMenuText)
            //{
            //    switch (choice) { }
            //}
            //else if (menuText == kanbanBoardMenuText)
            //{
            //    switch (choice) { }
            //}
            //else if (menuText == searchMenuText)
            //{
            //    switch (choice) { }
            //}
            else { return; }
        }
    }
}
