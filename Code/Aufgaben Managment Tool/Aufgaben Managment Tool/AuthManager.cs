namespace Aufgaben_Managment_Tool;

using Spectre.Console;

internal class AuthManager
{
    public User? LoggedInUser { get; private set; }



    public bool Login()
    {
        var repository = new UserRepository();
        var users = repository.LoadUsers();
        
        var username = AnsiConsole.Prompt<string>(
            new TextPrompt<string>("Bitte geben Sie Ihren Benutzernamen ein:")
            .PromptStyle("green"));
        
        var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        Console.WriteLine("Debug: Gefundener Benutzer: " + (user != null ? user.Username : "null"));
        if (user != null && user.ValidatePassword())
        {
            LoggedInUser = user;
            AnsiConsole.MarkupLine($"[green]Erfolgreich eingeloggt als {user.Username}.[/]");
            return true;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Ungültiger Benutzername oder Passwort.[/]");
            return false;
        }

    }
    public  void Logout()
    {
        if (LoggedInUser != null)
        {
            AnsiConsole.MarkupLine($"[yellow]Benutzer {LoggedInUser.Username} wurde abgemeldet.[/]");
            LoggedInUser = null;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Kein Benutzer ist derzeit angemeldet.[/]");
        }
    }
    public bool IsAdmin()
    {
        return LoggedInUser != null && LoggedInUser.Role == UserRole.Admin;
    }
    public void CreateUser()
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

        user.Role = UserRole.User;

        user.SetPassword();

        users.Add(user);
        repository.SaveUsers(users);

        AnsiConsole.MarkupLine($"[green]Benutzer {user.Username} mit Rolle {user.Role} wurde erstellt.[/]");
    }

}
