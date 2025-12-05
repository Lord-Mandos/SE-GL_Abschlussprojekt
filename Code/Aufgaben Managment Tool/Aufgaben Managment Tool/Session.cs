namespace Aufgaben_Managment_Tool
{
    internal static class Session
    {

        public static User? CurrentUser { get; set; }
    
    public static void StartSession()
        {
            CurrentUser = null;
            while (true)
            {
                if (CurrentUser == null) 
                {  
                    UIRenderer.UIMain(MenuSystem.StartMenu, "Startmenü");
                }
                else
                {
                    UIRenderer.UIMain(MenuSystem.mainMenuText, "Hauptmenü");
                }
            }
        }
    }
}