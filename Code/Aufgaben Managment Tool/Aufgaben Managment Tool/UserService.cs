using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class UserService
    {
        public static void CreateUserAdmin()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();

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
            repository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} mit Rolle {user.Role} wurde erstellt.[/]");
        }
        public static void GetUsers()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();
            var table = new Table();
            table.AddColumn("Benutzername");
            table.AddColumn("Rolle");
            foreach (var user in users)
            {
                table.AddRow(user.Username, user.Role.ToString());
            }
            AnsiConsole.Write(table);
        }
        public static void UpdateUser()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();
            var username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie den Benutzernamen des zu bearbeitenden Benutzers ein:"));

            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red]Benutzer nicht gefunden.[/]");
                return;
            }
            bool changeName = AnsiConsole.Confirm("Möchten Sie den Benutzernamen ändern?");


            if (changeName)
            {
                user.Username = AnsiConsole.Prompt<string>(
                    new TextPrompt<string>("Bitte geben Sie den neuen Benutzernamen ein:").PromptStyle("green").Validate(username =>
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
            }

            user.Role = AnsiConsole.Prompt<UserRole>(
                new SelectionPrompt<UserRole>()
                .Title("Bitte wählen Sie die neue Rolle:")
                .AddChoices(UserRole.Admin, UserRole.User));

            bool changePassword = AnsiConsole.Confirm("Möchten Sie das Passwort ändern?");
            if (changePassword)
                user.SetPassword();

            repository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} wurde aktualisiert.[/]");
        }
        public static void DeleteUser()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();

            var username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie den Benutzernamen des zu löschenden Benutzers ein:"));
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red]Benutzer nicht gefunden.[/]");
                return;
            }

            if (user.Role == UserRole.Admin)
            {
                AnsiConsole.MarkupLine("[red]Der Administrator kann nicht gelöscht werden![/]");
                return;
            }

            users.Remove(user);
            repository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} wurde gelöscht.[/]");
        }
    }
}
