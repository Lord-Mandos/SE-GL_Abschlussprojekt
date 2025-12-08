using System;
using System.Collections.Generic;
using System.Text;

namespace Aufgaben_Managment_Tool
{
    internal static class TaskRepository
    {
        private const string FilePath = "tasks.json";

        public static List<TaskItem> LoadAllTasks()
        {
            return StorageManager<TaskItem>.Load(FilePath) ?? new List<TaskItem>();
        }

        public static List<TaskItem> LoadTasks()
        {
            var tasks = LoadAllTasks();
            var usertasks = new List<TaskItem>();
            foreach (var _usertask in tasks)
            {
                if (Session.CurrentUser != null && _usertask.AssignedUser == Session.CurrentUser.Username)
                {
                    usertasks.Add(_usertask);
                }
            }
            return usertasks;
        }

        public static void SaveTasks(List<TaskItem> tasks)
        {
            if (Session.CurrentUser == null)
            {
                StorageManager<TaskItem>.Save(FilePath, tasks);
                return;
            }

            var all = LoadAllTasks();


            all.RemoveAll(t => t.AssignedUser == Session.CurrentUser.Username);

            foreach (var t in tasks)
            {
                if (string.IsNullOrWhiteSpace(t.AssignedUser))
                    t.AssignedUser = Session.CurrentUser.Username;
            }

            all.AddRange(tasks);
            StorageManager<TaskItem>.Save(FilePath, all);
        }
        public static void SaveAllTasks(List<TaskItem> allTasks)
        {
            StorageManager<TaskItem>.Save(FilePath, allTasks);
        }
    }
}
