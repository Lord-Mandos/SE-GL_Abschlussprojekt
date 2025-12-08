using Spectre.Console;
using System.Linq;

namespace Aufgaben_Managment_Tool
{
    internal class MenuSystem
    {
        public static List<Markup> mainMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Aufgabenverwaltung[/]"),
            new Markup("[green]>[/] [grey100]2. Kanban-Board[/]"),
            new Markup("[green]>[/] [grey100]3. Suchen[/]"),
            new Markup("[green]>[/] [grey100]4. Abmelden[/]"),
            new Markup("[green]>[/] [grey100]5. Benutzerverwaltung[/]"),
            new Markup("[green]>[/] [grey100]6. Beenden[/]")
        };
        public static List<Markup> taskMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Aufgabe erstellen[/]"),
            new Markup("[green]>[/] [grey100]2. Aufgabe anzeigen[/]"),
            new Markup("[green]>[/] [grey100]3. Aufgabe bearbeiten[/]"),
            new Markup("[green]>[/] [grey100]4. Aufgabe löschen[/]"),
            new Markup("[green]>[/] [grey100]5. Zurück[/]")
        };
        public static List<Markup> kanbanBoardMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Board anzeigen[/]"),
            new Markup("[green]>[/] [grey100]2. Aufgabe verschieben[/]"),
            new Markup("[green]>[/] [grey100]3. Zurück[/]")
        };
        public static List<Markup> searchMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Nach ID suchen[/]"),
            new Markup("[green]>[/] [grey100]2. Nach Titel suchen[/]"),
            new Markup("[green]>[/] [grey100]3. Zurück[/]")
        };
        public static List<Markup> userMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Benutzer erstellen[/]"),
            new Markup("[green]>[/] [grey100]2. Benutzer anzeigen[/]"),
            new Markup("[green]>[/] [grey100]3. Benutzer bearbeiten[/]"),
            new Markup("[green]>[/] [grey100]4. Benutzer löschen[/]"),
            new Markup("[green]>[/] [grey100]5. Zurück[/]")
        };
        public static List<Markup> StartMenu = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Login[/]"),
            new Markup("[green]>[/] [grey100]2. Registrierung[/]"),
            new Markup("[green]>[/] [grey100]3. Beenden[/]")
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
                Border = BoxBorder.Double,
                Padding = new Padding(3, 1, 7, 0),
                Expand = true
            };

            return panel;
        }

        public static void UpdateMainOverview(string action = "Menü geöffnet")
        {
            var tasks = TaskRepository.LoadTasks();
            var today = tasks.Count(t => t.DueDate.Date == DateTime.Now.Date);
            var open = tasks.Count(t => t.Status != TaskState.Done);
            var total = tasks.Count;
            var user = Session.CurrentUser?.Username ?? "Nicht angemeldet";

            BodyRightManager.SetTitle("Übersicht");
            BodyRightManager.Set(
                $"\nBenutzer:        {user}{Environment.NewLine}" +
                $"[grey100]Aufgaben heute:  {today}{Environment.NewLine}[/]" +
                $"Offene Aufgaben: {open}{Environment.NewLine}{Environment.NewLine}" +
                $"[grey100]Gesamt Aufgaben: {total}{Environment.NewLine}{Environment.NewLine}[/]"+
                $"Letzte Aktion:   {action}"
            );
        }

        public static void UserMenuChoice(List<Markup> menuText)
        {
            int choice = AnsiConsole.Prompt<int>(( new TextPrompt<int>("Bitte wählen Sie eine Option:").AddChoices<int>(Enumerable.Range(1,menuText.Count))));
            if (menuText == mainMenuText)
            {
                switch (choice)
                {
                    case 1:
                        UpdateMainOverview("Aufgabenverwaltung geöffnet");
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        UpdateMainOverview("Kanban-Board geöffnet");
                        UIRenderer.UIMain(kanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
                        UpdateMainOverview("Suchmenü geöffnet");
                        UIRenderer.UIMain(searchMenuText, "Suchen");
                        break;
                    case 4:
                        AuthManager.Logout();
                        break;
                    case 5:
                        if (Session.CurrentUser != null && Session.CurrentUser.Role == UserRole.Admin)
                        {
                            UpdateMainOverview("Benutzerverwaltung geöffnet");
                            UIRenderer.UIMain(userMenuText, "Benutzerverwaltung");
                        }
                        else
                        {
                            UpdateMainOverview("[red]Zugriff verweigert: Nur Administratoren dürfen die Benutzerverwaltung nutzen.[/]");
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
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        TaskService.ShowTasks();
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 3:
                        TaskService.updateTask();
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 4:
                        TaskService.deleteTask();
                        UIRenderer.UIMain(taskMenuText, "Aufgabenverwaltung");
                        break;
                    case 5:
                        UpdateMainOverview("Zurück zum Hauptmenü");
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
                        TaskService.ShowKanbanBoard();
                        UIRenderer.UIMain(kanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 2:
                        TaskService.changeTaskStatus();
                        UIRenderer.UIMain(kanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
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
                        UpdateMainOverview("Zurück zum Hauptmenü");
                        UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == StartMenu)
            {
                switch (choice)
                {
                    case 1:
                        if (AuthManager.Login())
                        {
                            UpdateMainOverview("Eingeloggt");
                            UIRenderer.UIMain(mainMenuText, "Hauptmenü");
                            break;
                        }
                        else
                        {
                            UIRenderer.UIMain(StartMenu, "Startmenü");
                        }
                        break;
                    case 2:
                        UserService.CreateUser();
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
