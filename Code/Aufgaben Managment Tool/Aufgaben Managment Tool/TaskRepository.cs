using System;
using System.Collections.Generic;
using System.Text;

namespace Aufgaben_Managment_Tool
{
    internal class TaskRepository
    {
        private const string FilePath = "tasks.json";
        public List<TaskItem> LoadTasks()
        {
           return StorageManager<TaskItem>.Load(FilePath);
        }
        public void SaveTasks(List<TaskItem> tasks)
        {
            StorageManager<TaskItem>.Save(FilePath, tasks);
        }
    }
}
