using Spectre.Console;
using System;
using System.Linq;

namespace Aufgaben_Managment_Tool
{
    internal static class HeaderFooterManager
    {
        public static Panel GetHeaderLeft()
        {
            var markup = new Markup("[bold]Programmname[/]");
            return new Panel(Align.Left(markup, VerticalAlignment.Middle))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1),
                Expand = true
            };
        }

        public static Panel GetHeaderRight()
        {
            var repo = new TaskRepository();
            var tasks = repo.LoadTasks();
            var todayCount = tasks.Count(t => t.DueDate.Date == DateTime.Now.Date);
            var openCount = tasks.Count(t => t.Status != TaskState.Done);

            var p1 = new Panel(new Markup($"[green]Anzahl aufg. Heute[/]\n[bold]{todayCount}[/]"))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = false
            };
            var p2 = new Panel(new Markup($"[green]Gesamt aufg. offen[/]\n[bold]{openCount}[/]"))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = false
            };

            var inner = new Layout("HeaderRightInner")
                .SplitColumns(
                    new Layout("HR_Left"),
                    new Layout("HR_Spacer"),
                    new Layout("HR_Right")
                );

            inner["HR_Spacer"].Ratio = 1;

            inner["HR_Left"].Update(p1);
            inner["HR_Spacer"].Update(new Markup("")); 
            inner["HR_Right"].Update(Align.Right(p2, VerticalAlignment.Middle));

            return new Panel(inner)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(0, 0),
                Expand = true
            };
        }

        public static Panel GetFooter()
        {
            var user = Session.CurrentUser?.Username ?? "Nicht angemeldet";
            var date = DateOnly.FromDateTime(DateTime.Now).ToString();

            var grid = new Grid().AddColumn().AddColumn();
            var left = new Markup($"User: [yellow]{user}[/]");
            var right = new Markup($"[grey]{date}[/]");

            grid.AddRow(left, Align.Right(right, VerticalAlignment.Middle));

            return new Panel(grid)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = true
            };
        }
    }
}