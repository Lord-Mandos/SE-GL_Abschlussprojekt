using Spectre.Console;

namespace Aufgaben_Managment_Tool
{
    internal class User
    {
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public string PasswordHash { get; private set; }
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
        public void ValidatePassword()
        {
            var password = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("Bitte geben Sie Ihr Passwort ein:")
                .PromptStyle("red")
                .Secret());
            PasswordHelper.VerifyPassword(password, this.Salt, this.PasswordHash);

        }

    }
}
