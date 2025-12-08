using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class MenuSystem
    {
        public static List<Markup> MainMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Aufgabenverwaltung[/]"),
            new Markup("[green]>[/] [grey100]2. Kanban-Board[/]"),
            new Markup("[green]>[/] [grey100]3. Abmelden[/]"),
            new Markup("[green]>[/] [grey100]4. Benutzerverwaltung[/]"),
            new Markup("[green]>[/] [grey100]5. Beenden[/]")
        };
        public static List<Markup> TaskMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Aufgabe erstellen[/]"),
            new Markup("[green]>[/] [grey100]2. Aufgaben anzeigen[/]"),
            new Markup("[green]>[/] [grey100]3. Aufgabe anzeigen[/]"),
            new Markup("[green]>[/] [grey100]4. Aufgabe bearbeiten[/]"),
            new Markup("[green]>[/] [grey100]5. Aufgabe löschen[/]"),
            new Markup("[green]>[/] [grey100]6. Zurück[/]")
        };
        public static List<Markup> KanbanBoardMenuText = new List<Markup>
        {
            new Markup("[green]>[/] [grey100]1. Board anzeigen[/]"),
            new Markup("[green]>[/] [grey100]2. Aufgabe verschieben[/]"),
            new Markup("[green]>[/] [grey100]3. Aufgabe anzeigen[/]"),
            new Markup("[green]>[/] [grey100]4. Zurück[/]")
        };

        public static List<Markup> UserMenuText = new List<Markup>
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
                $"[grey100]Gesamt Aufgaben: {total}{Environment.NewLine}{Environment.NewLine}[/]" +
                $"Letzte Aktion:   {action}"
            );
        }

        public static void UserMenuChoice(List<Markup> menuText)
        {
            int choice = AnsiConsole.Prompt<int>(( new TextPrompt<int>("Bitte wählen Sie eine Option:").AddChoices<int>(Enumerable.Range(1,menuText.Count))));
            if (menuText == MainMenuText)
            {
                switch (choice)
                {
                    case 1:
                        UpdateMainOverview("Aufgabenverwaltung geöffnet");
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        UpdateMainOverview("Kanban-Board geöffnet");
                        UIRenderer.UIMain(KanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
                        AuthManager.Logout();
                        break;
                    case 4:
                        if (Session.CurrentUser != null && Session.CurrentUser.Role == UserRole.Admin)
                        {
                            UpdateMainOverview("Benutzerverwaltung geöffnet");
                            UIRenderer.UIMain(UserMenuText, "Benutzerverwaltung");
                        }
                        else
                        {
                            UpdateMainOverview("[red]Zugriff verweigert: Nur Administratoren dürfen die Benutzerverwaltung nutzen.[/]");
                            UIRenderer.UIMain(MainMenuText, "Hauptmenü");
                        }
                        break;
                    case 5:
                        AnsiConsole.MarkupLine("[red]Programm wird beendet...[/]");
                        Environment.Exit(0);
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(MainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == TaskMenuText)
            {
                switch (choice)
                {
                    case 1:
                        TaskService.CreateTask();
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 2:
                        TaskService.ShowTasks();
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 3:
                        TaskService.ShowTaskDetails();
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 4:
                        TaskService.UpdateTask();
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 5:
                        TaskService.DeleteTask();
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                    case 6:
                        UpdateMainOverview("Zurück zum Hauptmenü");
                        UIRenderer.UIMain(MainMenuText, "Hauptmenü");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ungültige Auswahl. Bitte versuchen Sie es erneut.[/]");
                        UIRenderer.UIMain(TaskMenuText, "Aufgabenverwaltung");
                        break;
                }
            }
            else if (menuText == KanbanBoardMenuText)
            {
                switch (choice)
                {
                    case 1:
                        TaskService.ShowKanbanBoard();
                        UIRenderer.UIMain(KanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 2:
                        TaskService.ChangeTaskStatus();
                        UIRenderer.UIMain(KanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 3:
                        TaskService.ShowTaskDetails();
                        UIRenderer.UIMain(KanbanBoardMenuText, "Kanban-Board");
                        break;
                    case 4:
                        UpdateMainOverview("Zurück zum Hauptmenü");
                        UIRenderer.UIMain(MainMenuText, "Hauptmenü");
                        break;
                }
            }
            else if (menuText == UserMenuText)
            {
                switch (choice)
                {
                    case 1:
                        UserService.CreateUserAdmin();
                        UIRenderer.UIMain(UserMenuText, "Benutzerverwaltung");
                        break;
                    case 2:
                        UserService.GetUsers();
                        UIRenderer.UIMain(UserMenuText, "Benutzerverwaltung");
                        break;
                    case 3:
                        UserService.UpdateUser();
                        UIRenderer.UIMain(UserMenuText, "Benutzerverwaltung");
                        break;
                    case 4:
                        UserService.DeleteUser();
                        UIRenderer.UIMain(UserMenuText, "Benutzerverwaltung");
                        break;
                    case 5:
                        UpdateMainOverview("Zurück zum Hauptmenü");
                        UIRenderer.UIMain(MainMenuText, "Hauptmenü");
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
                            UIRenderer.UIMain(MainMenuText, "Hauptmenü");
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
