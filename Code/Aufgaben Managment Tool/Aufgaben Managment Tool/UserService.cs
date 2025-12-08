using Spectre.Console;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Aufgaben_Managment_Tool
{
    internal class UserService
    {
        public static void CreateUserAdmin()
        {
            var users = UserRepository.LoadUsers();

            var user = new User();

            user.Username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihren Benutzernamen ein:")
                .PromptStyle("green").Validate(username =>
                {
                    if (username.Length < 3)
                    {
                        return ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/]");
                    }

                    if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    {
                        return ValidationResult.Error("[red]Dieser Benutzername ist bereits vergeben.[/]");
                    }

                    return ValidationResult.Success();
                }));

            user.Role = AnsiConsole.Prompt<UserRole>(
                new SelectionPrompt<UserRole>()
                .Title("Bitte wählen Sie Ihre Rolle:")
                .AddChoices(UserRole.Admin, UserRole.User));

            user.SetPassword();

            users.Add(user);
            UserRepository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} mit Rolle {user.Role} wurde erstellt.[/]");

            var total = users.Count;
            var admins = users.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Benutzer erstellt: {user.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{user.Username}' mit Rolle {user.Role} erstellt"
            );

            UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
        }


        public static void EnsureInitialAdmin()
        {
            var users = UserRepository.LoadUsers();
            if (users == null || users.Count == 0)
            {

                BodyRightManager.SetTitle("Initiale Einrichtung");
                BodyRightManager.Set("Kein Benutzer gefunden. Bitte initialen Administrator anlegen.");
                UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");

                var admin = new User();

                admin.Username = AnsiConsole.Prompt<string>(
                    new TextPrompt<string>("[bold yellow]Initialer Administrator - Benutzername wählen:[/]")
                        .PromptStyle("green")
                        .Validate(name =>
                        {
                            return string.IsNullOrWhiteSpace(name) || name.Length < 3
                                ? ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/]")
                                : ValidationResult.Success();
                        }));

                admin.Role = UserRole.Admin;

                admin.SetPassword();

                users = new List<User> { admin };
                UserRepository.SaveUsers(users);

                TaskRepository.SaveTasks(new List<TaskItem>());

                BodyRightManager.SetTitle("Initialisierung abgeschlossen");
                BodyRightManager.Set($"Administrator '{admin.Username}' erstellt.");
            }
        }

        public static void CreateUser()
        {
            var users = UserRepository.LoadUsers();

            var user = new User();

            user.Username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte wählen Sie Ihren Benutzernamen (Registrierung):")
                .PromptStyle("green").Validate(username =>
                {
                    if (username.Length < 3)
                    {
                        return ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/]");
                    }

                    if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    {
                        return ValidationResult.Error("[red]Dieser Benutzername ist bereits vergeben.[/]");
                    }

                    return ValidationResult.Success();
                }));

            user.Role = UserRole.User;

            user.SetPassword();

            users.Add(user);
            UserRepository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Registrierung erfolgreich. Benutzer {user.Username} wurde mit Rolle {user.Role} angelegt.[/]");

            var total = users.Count;
            var admins = users.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Registrierung: {user.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{user.Username}' registriert"
            );

            UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");
        }

        public static void GetUsers()
        {
            var users = UserRepository.LoadUsers().OrderBy(u => u.Username).ToList();

            if (users.Count == 0)
            {
                BodyRightManager.SetTitle("Benutzerliste");
                BodyRightManager.Set("[grey]Keine Benutzer vorhanden[/]");
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            const int pageSize = 12;
            int page = 0;
            int pages = (users.Count + pageSize - 1) / pageSize;
            pages = Math.Max(1, pages);

            while (true)
            {
                var table = new Table().Expand();
                table.AddColumn(new TableColumn("[u]Benutzername[/]"));
                table.AddColumn(new TableColumn("[u]Rolle[/]"));

                var pageUsers = users.Skip(page * pageSize).Take(pageSize).ToList();
                foreach (var u in pageUsers)
                {
                    table.AddRow(u.Username, u.Role.ToString());
                }

                BodyRightManager.SetTitle($"Benutzerliste — Seite {page + 1}/{pages}");
                BodyRightManager.SetRenderable(table);
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");


                var actions = new List<string>();
                if (page > 0) actions.Add("← Zurück");
                if (page < pages - 1) actions.Add("Weiter →");
                const string backDisplay = "[grey]←[/] [yellow]Zurück zum Menü[/]";
                actions.Add(backDisplay);

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
                    MenuSystem.UpdateMainOverview("Benutzer angezeigt");
                    break;
                }
            }
        }
        public static void UpdateUser()
        {
            var users = UserRepository.LoadUsers().OrderBy(u => u.Username).ToList();

            if (users.Count == 0)
            {
                BodyRightManager.SetTitle("Benutzer bearbeiten");
                BodyRightManager.Set("[grey]Keine Benutzer vorhanden[/]");
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            const string backDisplay = "[grey]←[/] [yellow]Zurück[/]";

            var choices = users
                .Select(u => $"{u.Username}  ({u.Role})")
                .ToList();

            User userToEdit;
            string originalUsername;

            if (choices.Count == 1)
            {
                var singleLabel = choices[0];
                var edit = AnsiConsole.Confirm($"Einziger Benutzer: {singleLabel}. Möchten Sie diesen bearbeiten?");
                if (!edit)
                {
                    UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    return;
                }
                userToEdit = users[0];
            }
            else
            {
                choices.Add(backDisplay);

                var selected = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[bold yellow]Wähle den Benutzer der bearbeitet werden soll:[/]")
                        .PageSize(Math.Min(20, choices.Count))
                        .AddChoices(choices)
                );

                if (selected == backDisplay)
                {
                    UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    return;
                }

                var idx = choices.IndexOf(selected);
                if (idx < 0 || idx >= users.Count)
                {
                    AnsiConsole.MarkupLine("[red]Auswahl ungültig.[/]");
                    UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    return;
                }

                userToEdit = users[idx];
            }

            originalUsername = userToEdit.Username;

            bool changeName = AnsiConsole.Confirm("Möchten Sie den Benutzernamen ändern?");

            if (changeName)
            {
                userToEdit.Username = AnsiConsole.Prompt<string>(
                    new TextPrompt<string>("Bitte geben Sie den neuen Benutzernamen ein:").PromptStyle("green").Validate(newName =>
                    {
                        if (newName.Length < 3)
                        {
                            return ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/]");
                        }

                        var allUsers = UserRepository.LoadUsers();
                        if (allUsers.Any(u => u.Username.Equals(newName, StringComparison.OrdinalIgnoreCase) && !u.Username.Equals(originalUsername, StringComparison.OrdinalIgnoreCase)))
                        {
                            return ValidationResult.Error("[red]Dieser Benutzername ist bereits vergeben.[/]");
                        }
                        return ValidationResult.Success();
                    }));
            }

            var currentSessionUser = Session.CurrentUser;
            bool editingSelf = currentSessionUser != null && currentSessionUser.Username.Equals(originalUsername, StringComparison.OrdinalIgnoreCase);

            if (editingSelf)
            {
                AnsiConsole.MarkupLine("[yellow]Hinweis: Sie können Ihre eigene Administratorrolle nicht entfernen.[/]");
            }
            else
            {
                var newRole = AnsiConsole.Prompt<UserRole>(
                    new SelectionPrompt<UserRole>()
                    .Title("Bitte wählen Sie die neue Rolle:")
                    .AddChoices(UserRole.Admin, UserRole.User));

                if (userToEdit.Role == UserRole.Admin && newRole == UserRole.User)
                {
                    var adminCount = UserRepository.LoadUsers().Count(u => u.Role == UserRole.Admin);
                    if (adminCount <= 1)
                    {
                        AnsiConsole.MarkupLine("[red]Aktion abgebrochen: Es muss mindestens ein Administrator vorhanden sein.[/]");
                        BodyRightManager.SetTitle("Rollenänderung abgebrochen");
                        BodyRightManager.Set("Es ist nicht erlaubt, den letzten Administrator zu entziehen.");
                        UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    }
                    else
                    {
                        userToEdit.Role = newRole;
                    }
                }
                else
                {
                    userToEdit.Role = newRole;
                }
            }

            bool changePassword = AnsiConsole.Confirm("Möchten Sie das Passwort ändern?");
            if (changePassword)
                userToEdit.SetPassword();

            var all = UserRepository.LoadUsers();
            var indexInAll = all.FindIndex(u => u.Username.Equals(originalUsername, StringComparison.OrdinalIgnoreCase));
            if (indexInAll >= 0)
            {
                all[indexInAll] = userToEdit;
            }
            else
            {
                var idxNew = all.FindIndex(u => u.Username.Equals(userToEdit.Username, StringComparison.OrdinalIgnoreCase));
                if (idxNew >= 0)
                    all[idxNew] = userToEdit;
                else
                    all.Add(userToEdit);
            }
            UserRepository.SaveUsers(all);

            AnsiConsole.MarkupLine($"[green]Benutzer {userToEdit.Username} wurde aktualisiert.[/]");

            var total = all.Count;
            var admins = all.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Benutzer aktualisiert: {userToEdit.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{userToEdit.Username}' aktualisiert"
            );

            UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
        }
        public static void DeleteUser()
        {
            var users = UserRepository.LoadUsers().OrderBy(u => u.Username).ToList();

            if (users.Count == 0)
            {
                BodyRightManager.SetTitle("Benutzer löschen");
                BodyRightManager.Set("[grey]Keine Benutzer vorhanden[/]");
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            const string backDisplay = "[grey]←[/] [yellow]Zurück[/]";

            var choices = users
                .Select(u => $"{u.Username}  ({u.Role})")
                .ToList();

            if (choices.Count == 1)
            {
                var singleLabel = choices[0];
                var onlyUser = users[0];

                var confirm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Einziger Benutzer: {singleLabel}. Aktion wählen:")
                        .AddChoices("Löschen", "Zurück"));

                if (confirm == "Zurück")
                {
                    UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    return;
                }

                if (onlyUser.Role == UserRole.Admin)
                {
                    AnsiConsole.MarkupLine("[red]Der Administrator kann nicht gelöscht werden![/]");
                    BodyRightManager.SetTitle("Benutzer löschen");
                    BodyRightManager.Add("Löschversuch eines Administrators abgebrochen.");
                    UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                    return;
                }

                users.Remove(onlyUser);
                UserRepository.SaveUsers(users);

                AnsiConsole.MarkupLine($"[green]Benutzer {onlyUser.Username} wurde gelöscht.[/]");

                var total = users.Count;
                var admins = users.Count(u => u.Role == UserRole.Admin);
                BodyRightManager.SetTitle($"Benutzer gelöscht: {onlyUser.Username}");
                BodyRightManager.Set(
                    $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                    $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                    $"Letzte Aktion:{Environment.NewLine}- Benutzer '{onlyUser.Username}' gelöscht"
                );

                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            choices.Add(backDisplay);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold yellow]Wähle den Benutzer der gelöscht werden soll:[/]")
                    .PageSize(Math.Min(20, choices.Count))
                    .AddChoices(choices)
            );

            if (selected == backDisplay)
            {
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            var idx = choices.IndexOf(selected);
            if (idx < 0 || idx >= users.Count)
            {
                AnsiConsole.MarkupLine("[red]Auswahl ungültig.[/]");
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            var user = users[idx];

            if (user.Role == UserRole.Admin)
            {
                AnsiConsole.MarkupLine("[red]Der Administrator kann nicht gelöscht werden![/]");
                BodyRightManager.SetTitle("Benutzer löschen");
                BodyRightManager.Add("Löschversuch eines Administrators abgebrochen.");
                UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
                return;
            }

            users.Remove(user);
            UserRepository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} wurde gelöscht.[/]");

            var totalAfter = users.Count;
            var adminsAfter = users.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Benutzer gelöscht: {user.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {totalAfter}{Environment.NewLine}" +
                $"Administratoren: {adminsAfter}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{user.Username}' gelöscht"
            );

            UIRenderer.Refresh(MenuSystem.UserMenuText, "Benutzerverwaltung");
        }
    }
}
