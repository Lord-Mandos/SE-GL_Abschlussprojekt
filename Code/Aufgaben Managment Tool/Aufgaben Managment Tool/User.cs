using Spectre.Console;
using System.Text.Json.Serialization;

namespace Aufgaben_Managment_Tool
{
    internal class User
    {
        public string Username { get; set; }
        public UserRole Role { get; set; }
        [JsonInclude]
        public string PasswordHash { get; private set; }
        [JsonInclude]
        public byte[] Salt { get; private set; }

        public void SetPassword()
        {
            var password = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihr Passwort ein:")
                .PromptStyle("red")
                .Secret());
            Salt = PasswordHelper.GenerateSalt();
            PasswordHash = PasswordHelper.HashPassword(password, Salt);
        }
        public bool ValidatePassword()
        {
            var password = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihr Passwort ein:")
                .PromptStyle("red")
                .Secret());
            return PasswordHelper.VerifyPassword(password, this.Salt, this.PasswordHash);
        }
    }
}
