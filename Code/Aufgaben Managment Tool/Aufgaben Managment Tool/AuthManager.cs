using Spectre.Console;
using System;
using System.Linq;

namespace Aufgaben_Managment_Tool
{
    internal class AuthManager
    {
        public User? LoggedInUser => Session.CurrentUser;

        public bool Login()
        {
            var repository = new UserRepository();
            var users = repository.LoadUsers();

            BodyRightManager.SetTitle("Login");
            BodyRightManager.Set("Bitte geben Sie Ihren Benutzernamen ein:");
            UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");

            var username = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihren Benutzernamen ein:")
                .PromptStyle("green"));

            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine("Debug: Gefundener Benutzer: " + (user != null ? user.Username : "null"));

            if (user != null)
            {
                BodyRightManager.SetTitle("Login - Passwort");
                BodyRightManager.Set($"Benutzer: {user.Username}{Environment.NewLine}Bitte geben Sie Ihr Passwort ein:");
            }
            else
            {
                BodyRightManager.SetTitle("Login");
                BodyRightManager.Set($"Benutzer '{username}' nicht gefunden.{Environment.NewLine}Bitte versuchen Sie es erneut oder registrieren Sie sich.");
            }
            UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");

            if (user != null && user.ValidatePassword())
            {
                Session.CurrentUser = user;

                var taskRepo = new TaskRepository();
                var tasks = taskRepo.LoadTasks();
                var today = tasks.Count(t => t.DueDate.Date == DateTime.Now.Date);
                var open = tasks.Count(t => t.Status != TaskState.Done);

                BodyRightManager.SetTitle($"Willkommen, {user.Username}");
                BodyRightManager.Set(
                    $"Benutzer: {user.Username}{Environment.NewLine}" +
                    $"Rolle: {user.Role}{Environment.NewLine}{Environment.NewLine}" +
                    $"Aufgaben heute: {today}{Environment.NewLine}" +
                    $"Offene Aufgaben: {open}"
                );

                AnsiConsole.MarkupLine($"[green]Erfolgreich eingeloggt als {user.Username}.[/]");
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Ungültiger Benutzername oder Passwort.[/]");
                BodyRightManager.SetTitle("Login fehlgeschlagen");
                BodyRightManager.Set($"Letzte Aktion: Anmeldung fehlgeschlagen für '{username}'");
                UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");
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
                BodyRightManager.SetTitle("Abmeldung");
                BodyRightManager.Set("Sie wurden erfolgreich abgemeldet.");
                UIRenderer.Refresh(MenuSystem.StartMenu, "Startmenü");
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
