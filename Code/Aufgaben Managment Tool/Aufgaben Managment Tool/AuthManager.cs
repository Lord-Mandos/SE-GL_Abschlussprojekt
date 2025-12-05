using Spectre.Console;
using System;

namespace Aufgaben_Managment_Tool
{
    internal class AuthManager
    {
        public User? LoggedInUser => Session.CurrentUser;

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
                // In Session speichern, damit andere Klassen darauf zugreifen können
                Session.CurrentUser = user;
                AnsiConsole.MarkupLine($"[green]Erfolgreich eingeloggt als {user.Username}.[/]");
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Ungültiger Benutzername oder Passwort.[/]");
                return false;
            }
        }

        public void CreateUser()
        {
            UserService.CreateUser();
        }

        public void Logout()
        {
            if (Session.CurrentUser != null)
            {
                AnsiConsole.MarkupLine($"[yellow]Benutzer {Session.CurrentUser.Username} wurde abgemeldet.[/]");
                Session.CurrentUser = null;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Kein Benutzer ist derzeit angemeldet.[/]");
            }
        }

        public bool IsAdmin()
        {
            return Session.CurrentUser != null && Session.CurrentUser.Role == UserRole.Admin;
        }
    }
}
