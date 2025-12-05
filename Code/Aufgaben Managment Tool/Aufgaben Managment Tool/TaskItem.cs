using System;
using System.Collections.Generic;
using System.Text;

namespace Aufgaben_Managment_Tool
{
    internal class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DueDate { get; set; }
        public TaskState Status { get; set; }
        public string AssignedUser { get; set; }
    }
}
