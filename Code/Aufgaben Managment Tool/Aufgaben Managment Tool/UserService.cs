using Spectre.Console;
using System;
using System.Linq;

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

            UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
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
                UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
                return;
            }

            const int pageSize = 12;
            int page = 0;
            int pages = (users.Count + pageSize - 1) / pageSize;

            while (true)
            {
                var table = new Table().Expand();
                table.AddColumn(new TableColumn("[u]Benutzername[/]"));
                table.AddColumn(new TableColumn("[u]Rolle[/]"));

                var pageUsers = users.Skip(page * pageSize).Take(pageSize);
                foreach (var u in pageUsers)
                {
                    table.AddRow(u.Username, u.Role.ToString());
                }

                BodyRightManager.SetTitle($"Benutzerliste — Seite {page + 1}/{pages}");
                BodyRightManager.SetRenderable(table);
                UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");


                var actions = new List<string>();
                if (page > 0) actions.Add("← Zurück");
                if (page < pages - 1) actions.Add("Weiter →");
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

                    MenuSystem.UpdateMainOverview("Benutzer angezeigt");
                    break;
                }
            }
        }
        public static void UpdateUser()
        {
            var users = UserRepository.LoadUsers();
            var username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie den Benutzernamen des zu bearbeitenden Benutzers ein:"));

            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red]Benutzer nicht gefunden.[/]");
                BodyRightManager.SetTitle("Benutzer bearbeiten");
                BodyRightManager.Set($"Letzte Aktion: Bearbeiten fehlgeschlagen für '{username}'");
                UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
                return;
            }

            bool changeName = AnsiConsole.Confirm("Möchten Sie den Benutzernamen ändern?");

            if (changeName)
            {
                user.Username = AnsiConsole.Prompt<string>(
                    new TextPrompt<string>("Bitte geben Sie den neuen Benutzernamen ein:").PromptStyle("green").Validate(newName =>
                    {
                        if (newName.Length < 3)
                        {
                            return ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/>");
                        }

                        if (users.Any(u => u.Username.Equals(newName, StringComparison.OrdinalIgnoreCase) && !u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                        {
                            return ValidationResult.Error("[red]Dieser Benutzername ist bereits vergeben.[/>");
                        }
                        return ValidationResult.Success();
                    }));
            }

            var currentSessionUser = Session.CurrentUser;
            bool editingSelf = currentSessionUser != null && currentSessionUser.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase);

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

                
                if (user.Role == UserRole.Admin && newRole == UserRole.User)
                {
                    var adminCount = users.Count(u => u.Role == UserRole.Admin);
                    if (adminCount <= 1)
                    {
                        AnsiConsole.MarkupLine("[red]Aktion abgebrochen: Es muss mindestens ein Administrator vorhanden sein.[/]");
                        BodyRightManager.SetTitle("Rollenänderung abgebrochen");
                        BodyRightManager.Set("Es ist nicht erlaubt, den letzten Administrator zu entziehen.");
                        UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
                    }
                    else
                    {
                        user.Role = newRole;
                    }
                }
                else
                {
                    user.Role = newRole;
                }
            }

            bool changePassword = AnsiConsole.Confirm("Möchten Sie das Passwort ändern?");
            if (changePassword)
                user.SetPassword();

            UserRepository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} wurde aktualisiert.[/]");

            var total = users.Count;
            var admins = users.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Benutzer aktualisiert: {user.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{user.Username}' aktualisiert"
            );

            UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
        }
        public static void DeleteUser()
        {
            var users = UserRepository.LoadUsers();

            var username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie den Benutzernamen des zu löschenden Benutzers ein:"));
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red]Benutzer nicht gefunden.[/]");
                BodyRightManager.SetTitle("Benutzer löschen");
                BodyRightManager.Set($"Letzte Aktion: Löschversuch fehlgeschlagen für '{username}'");
                UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
                return;
            }

            if (user.Role == UserRole.Admin)
            {
                AnsiConsole.MarkupLine("[red]Der Administrator kann nicht gelöscht werden![/]");
                BodyRightManager.SetTitle("Benutzer löschen");
                BodyRightManager.Add("Löschversuch eines Administrators abgebrochen.");
                UIRenderer.Refresh(MenuSystem.userMenuText, "Benutzerverwaltung");
                return;
            }

            users.Remove(user);
            UserRepository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} wurde gelöscht.[/]");

            var total = users.Count;
            var admins = users.Count(u => u.Role == UserRole.Admin);
            BodyRightManager.SetTitle($"Benutzer gelöscht: {user.Username}");
            BodyRightManager.Set(
                $"Gesamt Benutzer: {total}{Environment.NewLine}" +
                $"Administratoren: {admins}{Environment.NewLine}{Environment.NewLine}" +
                $"Letzte Aktion:{Environment.NewLine}- Benutzer '{user.Username}' gelöscht"
            );
        }
    }
}
