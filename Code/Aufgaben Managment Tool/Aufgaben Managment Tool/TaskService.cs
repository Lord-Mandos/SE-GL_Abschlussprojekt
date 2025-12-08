using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aufgaben_Managment_Tool
{
    internal class TaskService
    {
        public static void createTask()
        {
            var tasks = TaskRepository.LoadTasks();

            var newTask = new TaskItem();

            newTask.Id = Guid.NewGuid();
            newTask.CreateAt = DateTime.Now;
            newTask.Status = TaskState.ToDo;
            newTask.AssignedUser = Session.CurrentUser?.Username ?? "Unbekannt";

            newTask.Title = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Aufgaben Titel eingeben:[/]")
                .PromptStyle("green")
                .Validate(title =>
                {
                    return title.Length < 3
                        ? ValidationResult.Error("[red]Der Titel muss mindestens 3 Zeichen lang sein.[/]")
                        : ValidationResult.Success();
                }));

            newTask.Description = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]kurze Aufgabenbeschreibung eingeben:[/]")
                .PromptStyle("green"));

            newTask.DueDate = AnsiConsole.Prompt<DateTime>(
                new TextPrompt<DateTime>("[bold yellow]Fälligkeitsdatum eingeben (Format: JJJJ-MM-TT):[/]")
                .PromptStyle("green"));

            tasks.Add(newTask);
            TaskRepository.SaveTasks(tasks);

            var total = tasks.Count;
            var open = tasks.Count(t => t.Status != TaskState.Done);
            var today = tasks.Count(t => t.CreateAt.Date == DateTime.Now.Date);

            BodyRightManager.SetTitle($"Aufgabe erstellt: {newTask.Title}");
            BodyRightManager.Set(
                $"Anzahl aufg. Heute: {today}{Environment.NewLine}" +
                $"Gesamt aufg. offen: {open}{Environment.NewLine}" +
                $"Gesamt Aufgaben: {total}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Aufgabe '{newTask.Title}' erstellt{Environment.NewLine}{Environment.NewLine}" +
                $"Fälligkeitsdatum: {newTask.DueDate:yyyy-MM-dd}{Environment.NewLine}" +
                $"Erstellt: {newTask.CreateAt:yyyy-MM-dd HH:mm}"
            );

            UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
        }

        public static void deleteTask()
        {

            var tasks = TaskRepository.LoadTasks();
            var taskTitle = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Geben Sie den Titel der zu löschenden Aufgabe ein:[/]")
                .PromptStyle("green"));

            var task = tasks.FirstOrDefault(t => t.Title.Equals(taskTitle, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                tasks.Remove(task);
                TaskRepository.SaveTasks(tasks);
                AnsiConsole.MarkupLine("[bold green]Aufgabe erfolgreich gelöscht![/]");

                var total = tasks.Count;
                var open = tasks.Count(t => t.Status != TaskState.Done);
                var today = tasks.Count(t => t.CreateAt.Date == DateTime.Now.Date);

                BodyRightManager.SetTitle($"Aufgabe gelöscht: {task.Title}");
                BodyRightManager.Set(
                    $"Anzahl aufg. Heute: {today}{Environment.NewLine}" +
                    $"Gesamt aufg. offen: {open}{Environment.NewLine}" +
                    $"Gesamt Aufgaben: {total}{Environment.NewLine}{Environment.NewLine}" +
                    $"Letzte Aktion:{Environment.NewLine}- Aufgabe '{task.Title}' gelöscht"
                );
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Aufgabe nicht gefunden![/]");
                BodyRightManager.SetTitle("Löschversuch fehlgeschlagen");
                BodyRightManager.Set($"Letzte Aktion: Löschversuch fehlgeschlagen für Titel '{taskTitle}'");
            }

            UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
        }

        public static void updateTask()
        {
            var tasks = TaskRepository.LoadTasks();

            var taskTitle = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Geben Sie den Titel der zu bearbeitenden Aufgabe ein:[/]")
                .PromptStyle("green"));
            var task = tasks.FirstOrDefault(t => t.Title.Equals(taskTitle, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                AnsiConsole.MarkupLine("[bold red]Aufgabe nicht gefunden![/]");
                BodyRightManager.SetTitle("Bearbeiten fehlgeschlagen");
                BodyRightManager.Set($"Letzte Aktion: Bearbeiten fehlgeschlagen für Titel '{taskTitle}'");
                UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
                return;
            }

            task.Title = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Neuen Aufgabentitel eingeben:[/]")
                .PromptStyle("green")
                .DefaultValue(task.Title)
                .Validate(title =>
                {
                    return title.Length < 3
                        ? ValidationResult.Error("[red]Der Titel muss mindestens 3 Zeichen lang sein.[/]")
                        : ValidationResult.Success();
                }));

            task.Description = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Neue Aufgabenbeschreibung eingeben:[/]")
                .PromptStyle("green")
                .DefaultValue(task.Description));

            task.DueDate = AnsiConsole.Prompt<DateTime>(
                new TextPrompt<DateTime>("[bold yellow]Neues Fälligkeitsdatum eingeben (Format: JJJJ-MM-TT):[/]")
                .PromptStyle("green")
                .DefaultValue(task.DueDate));

            task.Status = AnsiConsole.Prompt<TaskState>(
                new SelectionPrompt<TaskState>()
                .Title("[bold yellow]Neuen Aufgabenstatus auswählen:[/]")
                .AddChoices(TaskState.ToDo, TaskState.InProgress, TaskState.Done));

            TaskRepository.SaveTasks(tasks);
            AnsiConsole.MarkupLine("[bold green]Aufgabe erfolgreich aktualisiert![/]");

            var total = tasks.Count;
            var open = tasks.Count(t => t.Status != TaskState.Done);
            var today = tasks.Count(t => t.CreateAt.Date == DateTime.Now.Date);

            BodyRightManager.SetTitle($"Aufgabe aktualisiert: {task.Title}");
            MenuSystem.UpdateMainOverview
            (
                $"{Environment.NewLine}- Aufgabe '{task.Title}' aktualisiert{Environment.NewLine}{Environment.NewLine}" +
                $"Status: {task.Status}{Environment.NewLine}" +
                $"Fällig: {task.DueDate:yyyy-MM-dd}"
            );

            UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
        }

        public static void ShowTasks()
        {
            var tasks = TaskRepository.LoadTasks().OrderBy(t => t.DueDate).ToList();
            tasks.Reverse();

            if (tasks.Count == 0)
            {
                BodyRightManager.SetTitle("Aufgaben");
                BodyRightManager.Set("[grey]Keine Aufgaben vorhanden[/]");
                UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
                return;
            }

            const int pageSize = 6;
            int page = 0;
            int pages = (tasks.Count + pageSize - 1) / pageSize;

            while (true)
            {
                var pageTasks = tasks.Skip(page * pageSize).Take(pageSize).ToList();

                var grid = new Grid().AddColumn().AddColumn().AddColumn();
                grid.Centered();

                int totalHeight = Console.WindowHeight;
                int bodyHeight = (int)(totalHeight * 0.60);
                int cellHeight = (int)(bodyHeight * 0.45);

                int panelTextWidth = (int)(Math.Max(0, ((Console.WindowWidth * 0.6) / 3) - 4));
                int maxTextLines = cellHeight - 5;

                string WrapAndTruncateText(string text, int maxCharsPerLine, int maxLines)
                {
                    if (string.IsNullOrWhiteSpace(text)) return string.Empty;

                    var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var lines = new List<string>();
                    var current = "";

                    void PushCurrent()
                    {
                        if (!string.IsNullOrEmpty(current))
                        {
                            lines.Add(current);
                            current = "";
                        }
                    }

                    foreach (var w in words)
                    {
                        if (w.Length > maxCharsPerLine)
                        {
                            PushCurrent();
                            if (lines.Count == maxLines) break;

                            for (int i = 0; i < w.Length; i += maxCharsPerLine)
                            {
                                if (lines.Count == maxLines) break;
                                var len = Math.Min(maxCharsPerLine, w.Length - i);
                                var part = w.Substring(i, len);
                                lines.Add(part);
                            }

                            if (lines.Count == maxLines) break;
                        }
                        else
                        {
                            if (current.Length == 0)
                            {
                                current = w;
                            }
                            else if (current.Length + 1 + w.Length <= maxCharsPerLine)
                            {
                                current += " " + w;
                            }
                            else
                            {
                                lines.Add(current);
                                current = w;
                                if (lines.Count == maxLines) break;
                            }
                        }

                        if (lines.Count == maxLines) break;
                    }

                    if (lines.Count < maxLines)
                    {
                        PushCurrent();
                    }

                    if (lines.Count > maxLines)
                    {
                        lines = lines.Take(maxLines).ToList();
                    }

                    var usedWordsCount = lines.SelectMany(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Count();
                    if (usedWordsCount < words.Length)
                    {
                        var last = lines.Last();
                        if (last.Length > 3)
                            last = last.Substring(0, Math.Max(0, last.Length - 3)) + "...";
                        lines[lines.Count - 1] = last;
                    }

                    return string.Join(Environment.NewLine, lines.Take(maxLines));
                }

                Panel CreateTaskPanel(TaskItem t)
                {
                    var truncatedDesc = WrapAndTruncateText(t.Description ?? string.Empty, panelTextWidth, maxTextLines);
                    var contentText = $"[bold yellow]{t.Title}[/]{Environment.NewLine}" +
                                      $"{truncatedDesc}{Environment.NewLine}{Environment.NewLine}" +
                                      $"[grey]Fällig: {t.DueDate:yyyy-MM-dd}[/]";
                    var raw = new Markup(contentText);
                    var centered = Align.Center(raw, VerticalAlignment.Top);
                    return new Panel(centered)
                    {
                        Height = cellHeight,
                        Border = BoxBorder.Rounded,
                        Padding = new Padding(0, 0),
                        Expand = false
                    };
                }

                var cellsRow1 = new List<IRenderable>();
                for (int i = 0; i < 3; i++)
                {
                    if (i < pageTasks.Count)
                        cellsRow1.Add(CreateTaskPanel(pageTasks[i]));
                    else
                        cellsRow1.Add(new Panel(Align.Center(new Markup("[grey]Keine weitere Aufgabe![/]"), VerticalAlignment.Middle)) { Height = cellHeight, Padding = new Padding(1, 0), Border = BoxBorder.Rounded, Expand = false });
                }
                grid.AddRow(cellsRow1[0], cellsRow1[1], cellsRow1[2]);

                var cellsRow2 = new List<IRenderable>();
                for (int i = 3; i < 6; i++)
                {
                    int idx = i;
                    if (idx < pageTasks.Count)
                        cellsRow2.Add(CreateTaskPanel(pageTasks[idx]));
                    else
                        cellsRow2.Add(new Panel(Align.Center(new Markup("[grey]Keine weitere Aufgabe![/]"), VerticalAlignment.Middle)) { Height = cellHeight, Padding = new Padding(1, 0), Border = BoxBorder.Rounded, Expand = false });
                }
                grid.AddRow(cellsRow2[0], cellsRow2[1], cellsRow2[2]);

                BodyRightManager.SetTitle($"Aufgaben — Seite {page + 1}/{pages}");
                BodyRightManager.SetRenderable(grid);
                UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");

                var actions = new List<string>();
                if (page < pages - 1) actions.Add("Weiter →");
                if (page > 0) actions.Add("← Zurück");
                actions.Add("Zurück zum Menü");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wähle Aktion:")
                        .AddChoices(actions));

                if (choice == "Weiter →")
                {
                    page++;
                    continue;
                }
                if (choice == "← Zurück")
                {
                    page--;
                    continue;
                }

                MenuSystem.UpdateMainOverview("Aufgaben angezeigt");
                //UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
                break;
            }
        }

        public static void ShowKanbanBoard()
        {
            var tasks = TaskRepository.LoadTasks().OrderBy(t => t.DueDate).ToList();

            var todo = tasks.Where(t => t.Status == TaskState.ToDo).ToList();
            var inProgress = tasks.Where(t => t.Status == TaskState.InProgress).ToList();
            var done = tasks.Where(t => t.Status == TaskState.Done).ToList();

            // Berechne, wie viele Einträge pro Spalte in den sichtbaren Body passen
            int totalHeight = Console.WindowHeight;
            int bodyHeight = (int)(totalHeight * 0.60);
            // jede Aufgabe benötigt ungefähr 2 Zeilen (Titel + Fälligkeitsdatum)
            // Rand/Überschrift/Borders abziehen -> -4
            int rowsPerColumn = Math.Max(1, (bodyHeight - 5) / 2);

            int maxColumnItems = Math.Max(todo.Count, Math.Max(inProgress.Count, done.Count));
            int pages = (maxColumnItems + rowsPerColumn - 1) / rowsPerColumn;

            int page = 0;

            while (true)
            {
                var todoPage = todo.Skip(page * rowsPerColumn).Take(rowsPerColumn).ToList();
                var inProgPage = inProgress.Skip(page * rowsPerColumn).Take(rowsPerColumn).ToList();
                var donePage = done.Skip(page * rowsPerColumn).Take(rowsPerColumn).ToList();

                var table = new Table().Border(TableBorder.Rounded).Expand();
                table.AddColumn(new TableColumn("[bold]To Do[/]").Centered());
                table.AddColumn(new TableColumn("[bold]In Progress[/]").Centered());
                table.AddColumn(new TableColumn("[bold]Done[/]").Centered());

                for (int r = 0; r < rowsPerColumn; r++)
                {
                    string cellTodo = r < todoPage.Count ? $"[bold yellow]{todoPage[r].Title}[/]\n[grey]Fällig: {todoPage[r].DueDate:yyyy-MM-dd}[/]" : "";
                    string cellInProg = r < inProgPage.Count ? $"[bold yellow]{inProgPage[r].Title}[/]\n[grey]Fällig: {inProgPage[r].DueDate:yyyy-MM-dd}[/]" : "";
                    string cellDone = r < donePage.Count ? $"[bold yellow]{donePage[r].Title}[/]\n[grey]Fällig: {donePage[r].DueDate:yyyy-MM-dd}[/]" : "";
                
                    table.AddRow(cellTodo, cellInProg, cellDone);
                }

                BodyRightManager.SetTitle($"Kanban-Board — Seite {page + 1}/{pages}");
                BodyRightManager.SetRenderable(table);
                UIRenderer.Refresh(MenuSystem.kanbanBoardMenuText, "Kanban-Board");

                var actions = new List<string>();
                if (page < pages - 1) actions.Add("Weiter →");
                if (page > 0) actions.Add("← Zurück");
                actions.Add("Zurück zum Menü");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wähle Aktion:")
                        .AddChoices(actions));

                if (choice == "Weiter →")
                {
                    page++;
                    continue;
                }
                else if (choice == "← Zurück")
                {
                    page--;
                    continue;
                }
                else
                {
                    MenuSystem.UpdateMainOverview("Kanban-Board angezeigt");
                    break;
                }
            }
        }

        public static void changeTaskStatus()
        {
            var tasks = TaskRepository.LoadTasks().OrderBy(t => t.DueDate).ToList();
            if (tasks.Count == 0)
            {
                BodyRightManager.SetTitle("Kanban - Verschieben");
                BodyRightManager.Set("[grey]Keine Aufgaben vorhanden[/]");
                UIRenderer.Refresh(MenuSystem.kanbanBoardMenuText, "Kanban-Board");
                return;
            }

            var choices = tasks
                .Select(t => $"{t.Title}")
                .ToList();

            var selectedLabel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wähle die Aufgabe, die du verschieben möchtest:")
                    .AddChoices(choices));

            int idx = choices.IndexOf(selectedLabel);

            var task = tasks[idx];

            var newStatus = AnsiConsole.Prompt<TaskState>(
                new SelectionPrompt<TaskState>()
                    .Title($"Neuen Status für '{task.Title}' wählen (aktuell: {task.Status}):")
                    .AddChoices(TaskState.ToDo, TaskState.InProgress, TaskState.Done)
                    .PageSize(3));

            if (newStatus == task.Status)
            {
                AnsiConsole.MarkupLine("[yellow]Status unverändert.[/]");
                BodyRightManager.SetTitle("Verschieben abgebrochen");
                BodyRightManager.Set($"Aufgabe '{task.Title}' bleibt im Status {task.Status}.");
                UIRenderer.Refresh(MenuSystem.kanbanBoardMenuText, "Kanban-Board");
                return;
            }

            task.Status = newStatus;
            TaskRepository.SaveTasks(tasks);

            MenuSystem.UpdateMainOverview(
                $"Aufgabe '{task.Title}' verschoben von {task.Status} zu {newStatus}.{Environment.NewLine}Fällig: {task.DueDate:yyyy-MM-dd}");
            AnsiConsole.MarkupLine("[green]Status erfolgreich geändert.[/]");
            UIRenderer.Refresh(MenuSystem.kanbanBoardMenuText, "Kanban-Board");
        }
    }
}
