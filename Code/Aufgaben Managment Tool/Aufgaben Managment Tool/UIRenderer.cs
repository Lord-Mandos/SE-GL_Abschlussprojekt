using Spectre.Console;
using System;

namespace Aufgaben_Managment_Tool
{
    internal class UIRenderer
    {
        public static void UIMain(List<Markup> menuText, string menuTitle)
        {
            Refresh(menuText, menuTitle);
            MenuSystem.UserMenuChoice(menuText);
        }

        public static void Refresh(List<Markup> menuText, string menuTitle)
        {
            AnsiConsole.Clear();
            int totalHeight = Console.WindowHeight;

            int headerHeight = (int)(totalHeight * 0.20);
            int bodyHeight = (int)(totalHeight * 0.60);
            int footerHeight = (int)(totalHeight * 0.15);

            var mainLayout = new Layout("Window")
                .SplitRows(
                    new Layout("Header").SplitColumns(
                        new Layout("HeaderLeft"),
                        new Layout("HeaderRight")),
                    new Layout("Body").SplitColumns(
                        new Layout("BodyLeft"),
                        new Layout("BodyRight")),
                    new Layout("Footer"));

            mainLayout["Header"].Size = headerHeight;
            mainLayout["Body"].Size = bodyHeight;
            mainLayout["Footer"].Size = footerHeight;

            mainLayout["BodyRight"].Ratio = 2;
            mainLayout["HeaderRight"].Ratio = 2;

            mainLayout["HeaderLeft"].Update(HeaderFooterManager.GetHeaderLeft());
            mainLayout["HeaderRight"].Update(HeaderFooterManager.GetHeaderRight());

            mainLayout["BodyLeft"].Update(MenuSystem.MenuPanel(menuText, menuTitle));

            mainLayout["BodyRight"].Update(BodyRightManager.GetPanel());

            mainLayout["Footer"].Update(HeaderFooterManager.GetFooter());

            AnsiConsole.Write(mainLayout);
        }
    }
}