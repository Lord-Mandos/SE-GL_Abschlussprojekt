namespace Aufgaben_Managment_Tool
{
    internal static class Session
    {
        public static User? CurrentUser { get; set; }

        public static void StartSession()
        {
            CurrentUser = null;

            BodyRightManager.SetTitle("Willkommen");
            BodyRightManager.Set(
                "Willkommen bei [bold yellow]TaskHub[/]." + Environment.NewLine +
                "Wähle 'Login' um dich anzumelden oder 'Registrierung' um einen neuen Benutzer anzulegen." + Environment.NewLine +
                "Benutze die Nummern im Menü zur Navigation."
            );

            while (true)
            {
                if (CurrentUser == null)
                {
                    UIRenderer.UIMain(MenuSystem.StartMenu, "Startmenü");
                }
                else
                {
                    MenuSystem.UpdateMainOverview();
                    UIRenderer.UIMain(MenuSystem.mainMenuText, "Hauptmenü");
                }
            }
        }
    }
}