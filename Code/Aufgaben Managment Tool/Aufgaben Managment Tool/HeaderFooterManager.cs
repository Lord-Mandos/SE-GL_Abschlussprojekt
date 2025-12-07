using Spectre.Console;


namespace Aufgaben_Managment_Tool
{
    internal static class HeaderFooterManager
    {
        public static Panel GetHeaderLeft()
        {
            var title = new Markup("[bold red]TaskHub[/]");
            return new Panel(Align.Center(title, VerticalAlignment.Middle))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = true
            };
        }

        public static Panel GetHeaderRight()
        {
            var repo = new TaskRepository();
            var tasks = repo.LoadTasks();
            var todayCount = tasks.Count(t => t.DueDate.Date == DateTime.Now.Date);
            var openCount = tasks.Count(t => t.Status != TaskState.Done);

            var p1 = new Panel(new Markup($"[yellow]Aufgaben Heute[/]\n[bold]{todayCount}[/]").Justify(Justify.Center))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = false
            };
            var p2 = new Panel(new Markup($"[yellow]Gesamt offen[/]\n[bold]{openCount}[/]").Justify(Justify.Center))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                Expand = false
            };

            var inner = new Layout("HeaderRightInner")
                .SplitColumns(
                    new Layout("HR_Left"),
                    new Layout("HR_Right")
                );


            inner["HR_Left"].Update(Align.Left(p1, VerticalAlignment.Middle));
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