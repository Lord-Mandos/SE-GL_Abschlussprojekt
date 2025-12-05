using Spectre.Console;
using System;
using System.Linq;

namespace Aufgaben_Managment_Tool
{
    internal class TaskService
    {
        public static void createTask()
        {
            var _taskRepository = new TaskRepository();

            var tasks = _taskRepository.LoadTasks();

            var newTask = new TaskItem();

            newTask.Id = Guid.NewGuid();
            newTask.CreateAt = DateTime.Now;
            newTask.Status = TaskState.ToDo;
            //newTask.AssignedUser =  Assigned User wir hier festgelegt über eingeloggten User

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
            _taskRepository.SaveTasks(tasks);

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
            var _taskRepository = new TaskRepository();
            var tasks = _taskRepository.LoadTasks();
            var taskTitle = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[bold yellow]Geben Sie den Titel der zu löschenden Aufgabe ein:[/]")
                .PromptStyle("green"));

            var task = tasks.FirstOrDefault(t => t.Title.Equals(taskTitle, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                tasks.Remove(task);
                _taskRepository.SaveTasks(tasks);
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

            var _taskRepository = new TaskRepository();
            var tasks = _taskRepository.LoadTasks();

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

            _taskRepository.SaveTasks(tasks);
            AnsiConsole.MarkupLine("[bold green]Aufgabe erfolgreich aktualisiert![/]");

            var total = tasks.Count;
            var open = tasks.Count(t => t.Status != TaskState.Done);
            var today = tasks.Count(t => t.CreateAt.Date == DateTime.Now.Date);

            BodyRightManager.SetTitle($"Aufgabe aktualisiert: {task.Title}");
            BodyRightManager.Set(
                $"Anzahl aufg. Heute: {today}{Environment.NewLine}" +
                $"Gesamt aufg. offen: {open}{Environment.NewLine}" +
                $"Gesamt Aufgaben: {total}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Aufgabe '{task.Title}' aktualisiert{Environment.NewLine}{Environment.NewLine}" +
                $"Status: {task.Status}{Environment.NewLine}" +
                $"Fällig: {task.DueDate:yyyy-MM-dd}"
            );

            UIRenderer.Refresh(MenuSystem.taskMenuText, "Aufgabenverwaltung");
        }
    }
}
