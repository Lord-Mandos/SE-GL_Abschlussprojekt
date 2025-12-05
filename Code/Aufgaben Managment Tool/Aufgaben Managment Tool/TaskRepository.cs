using System;
using System.Collections.Generic;
using System.Text;

namespace Aufgaben_Managment_Tool
{
    internal class TaskRepository
    {
        private const string FilePath = "tasks.json";
        public List<Task> LoadTasks()
        {
           return StorageManager<Task>.Load(FilePath);
        }
        public void SaveTasks(List<Task> tasks)
        {
            StorageManager<Task>.Save(FilePath, tasks);
        }
    }
}
