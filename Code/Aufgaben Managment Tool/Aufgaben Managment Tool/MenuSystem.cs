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
            new Markup("[green]>[/] 5. Benutzerverwaltung"),
            new Markup("[green]>[/] 6. Beenden")
        };
        public static List<Markup> taskMenuText = new List<Markup>
        {
            new Markup("[green]>[/] 1. Aufgabe erstellen"),
            new Markup("[green]>[/] 2. Aufgabe anzeigen"),
            new Markup("[green]>[/] 3. Aufgabe bearbeiten"),
            new Markup("[green]>[/] 4. Aufgabe löschen"),
            new Markup("[green]>[/] 5. Zurück")
        };
        public static List<Markup> kanbanBoardMenuText = new List<Markup>
        {
            new Markup("[green]>[/] 1. Board anzeigen"),
            new Markup("[green]>[/] 2. Board bearbeiten"),
            new Markup("[green]>[/] 3. Board löschen"),
            new Markup("[green]>[/] 4. Zurück")
        };
        public static List<Markup> searchMenuText = new List<Markup>
        {
            new Markup("[green]>[/] 1. Nach ID suchen"),
            new Markup("[green]>[/] 2. Nach Titel suchen"),
            new Markup("[green]>[/] 3. Zurück")
        };
        public static List<Markup> userMenuText = new List<Markup>
        {
            new Markup("[green]>[/] 1. Benutzer erstellen"),
            new Markup("[green]>[/] 2. Benutzer anzeigen"),
            new Markup("[green]>[/] 3. Benutzer bearbeiten"),
            new Markup("[green]>[/] 4. Benutzer löschen"),
            new Markup("[green]>[/] 5. Zurück")
        };
        public static List<Markup> StartMenu = new List<Markup>
        {
            new Markup("[green]>[/] 1. Login"),
            new Markup("[green]>[/] 2. Registrierung"),
            new Markup("[green]>[/] 3. Beenden")
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
                Padding = new Padding(3, 1, 7, 0),
                Expand = true
            };

            return panel;
        }

        public static void UserMenuChoice(List<Markup> menuText)
        {
            int choice = AnsiConsole.Prompt(
            new TextPrompt<int>("\"Bitte wählen Sie eine Option:\"")
            .AddChoices(Enumerable.Range(1, menuText.Count)));

            if (menuText == mainMenuText)
            {
                switch (choice)
                {
                    case 1:
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        UIRenderer.UIMain(kanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
                        UIRenderer.UIMain(searchMenuText, "Suchen");
                        break;
                    case 4:
                        new AuthManager().Logout();
                        UIRenderer.UIMain(StartMenu, "Startmenü");
                        break;
                    case 5:
                        if (Session.CurrentUser != null && Session.CurrentUser.Role == UserRole.Admin)
                        {
                            UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Zugriff verweigert: Nur Administratoren dürfen die Benutzerverwaltung nutzen.[/]");
                            UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        }
                        break;
                    case 6:
                        AnsiConsole.MarkupLine("[red]Programm wird beendet...[/]");
                        Environment.Exit(0);
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == taskMenuText)
            {
                switch (choice)
                {
                    case 1:
                        TaskService.createTask();
                        break;
                    case 2:
                        // Aufgabe anzeigen logic here
                        break;
                    case 3:
                        TaskService.updateTask();
                        break;
                    case 4:
                        TaskService.deleteTask();
                        break;
                    case 5:
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                }
            }
            else if (menuText == kanbanBoardMenuText)
            {
                switch (choice)
                {
                    case 1:
                        // Board anzeigen logic here
                        break;
                    case 2:
                        // Board bearbeiten logic here
                        break;
                    case 3:
                        // Board löschen logic here
                        break;
                    case 4:
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == searchMenuText)
            {
                switch (choice)
                {
                    case 1:
                        // Nach ID suchen logic here
                        break;
                    case 2:
                        // Nach Titel suchen logic here
                        break;
                    case 3:
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == userMenuText)
            {
                switch (choice)
                {
                    case 1:
                        UserService.CreateUserAdmin();
                        UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        break;
                    case 2:
                        UserService.GetUsers();
                        UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        break;
                    case 3:
                        UserService.UpdateUser();
                        UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        break;
                    case 4:
                        UserService.DeleteUser();
                        UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        break;
                    case 5:
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == StartMenu)
            {
                switch (choice)
                {
                    case 1:
                        var auth = new AuthManager();
                        if (auth.Login())
                        {
                            UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        }
                        else
                        {
                            UIRenderer.UIMain(StartMenu, "Startmenü");
                        }
                        break;
                    case 2:
                        new AuthManager().CreateUser();
                        UIRenderer.UIMain(StartMenu, "Startmenü");
                        break;
                    case 3:
                        AnsiConsole.MarkupLine("[red]Programm wird beendet...[/]");
                        Environment.Exit(0);
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(StartMenu, "Startmenü");
                        break;
                }
            }
            else { return; }
        }
    }
}
