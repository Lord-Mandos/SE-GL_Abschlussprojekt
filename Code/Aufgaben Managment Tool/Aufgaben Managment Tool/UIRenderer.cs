using Spectre.Console;
using System;

namespace Aufgaben_Managment_Tool
{
    internal class UIRenderer
    {
        private readonly static Markup headerLeftMarkup =
            new Markup($"Programm name");

        private readonly static Layout HeaderLeft =
            new Layout("HeaderLeft").Update(
                new Panel(Align.Left(headerLeftMarkup, VerticalAlignment.Middle)).Expand());

        private readonly static Markup headerRightMarkup =
            new Markup($"{DateOnly.FromDateTime(DateTime.Now)}");

        private readonly static Layout HeaderRight =
            new Layout("HeaderRight").Update(
                new Panel(Align.Right(headerRightMarkup, VerticalAlignment.Middle)).Expand());

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

            var headerLeft = new Panel(Align.Left(new Markup("Programm name"), VerticalAlignment.Middle)).Expand();
            var headerRight = new Panel(Align.Right(new Markup($"{DateOnly.FromDateTime(DateTime.Now)}"), VerticalAlignment.Middle)).Expand();

            mainLayout["HeaderLeft"].Update(headerLeft);
            mainLayout["HeaderRight"].Update(headerRight);

            mainLayout["BodyLeft"].Update(MenuSystem.MenuPanel(menuText, menuTitle));

            mainLayout["BodyRight"].Update(BodyRightManager.GetPanel());

            AnsiConsole.Write(mainLayout);
        }
    }
}