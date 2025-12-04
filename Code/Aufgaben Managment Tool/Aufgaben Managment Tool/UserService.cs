using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class UserService
    {
        public static void CreateUser()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();

            var user = new User();

            user.Username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihren Benutzernamen ein:")
                .PromptStyle("green").Validate(username =>
                {
                    return username.Length >= 3
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Der Benutzername muss mindestens 3 Zeichen lang sein.[/]");
                }));

            user.Role = AnsiConsole.Prompt<UserRole>(
                new SelectionPrompt<UserRole>()
                .Title("Bitte wählen Sie Ihre Rolle:")
                .AddChoices(UserRole.admin, UserRole.user));

            user.SetPassword();

            users.Add(user);
            repository.SaveUsers(users);

            AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} mit Rolle {user.Role} wurde erstellt.[/]");
        }


    }
}
