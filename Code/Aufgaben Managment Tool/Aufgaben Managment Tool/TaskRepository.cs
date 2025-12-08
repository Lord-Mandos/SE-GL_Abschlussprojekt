using System;
using System.Collections.Generic;
using System.Text;

namespace Aufgaben_Managment_Tool
{
    internal static class TaskRepository
    {
        private const string FilePath = "tasks.json";

        public static List<TaskItem> LoadTasks()
        {
            var tasks = StorageManager<TaskItem>.Load(FilePath);
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
            StorageManager<TaskItem>.Save(FilePath, tasks);
        }
    }
}
