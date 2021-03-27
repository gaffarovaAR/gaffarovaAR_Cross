using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GaffarovaAlbina.Models
{
    public class Assignment
    {
        public long Id { get; set; }
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        public List<Executor> Executors { get; set; }
        public List<Attempt> History { get; set; }
        public bool Done { get; set; } = false;
        public bool is_Overdue => (Deadline.Subtract(DateTime.Now).TotalSeconds < 0);
        public TimeSpan RemainingDays => Deadline.Subtract(DateTime.Now);
        public DateTime DateLastChange()
        {
            if (History != null && History.Count != 0)
                return History[History.Count - 1].DateChange;
            else
                return DateCreate;
        }

        [DataType(DataType.Date)]
        public DateTime DateCreate { get; set; }

        public Assignment() 
        {
        }
        public Assignment(AssignmentDTO assignmentDTO, List<Executor> executors)
        {
            Text = assignmentDTO.Text;
            Deadline = DateTime.Parse(assignmentDTO.Deadline);
            Executors = executors;
        }
    }

    public class AssignmentDTO
    {
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public string Deadline { get; set; }
        public List<long> ExecutorsID { get; set; }
    }

    public class AssignmentVM
    {
        public long Id { get; set; }
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        public string ExecutorsList { get; set; }
        public string Done { get; set; }
        public string is_Overdue()
        {
            if (Deadline.Subtract(DateTime.Now).TotalSeconds < 0)
            {
                return $"Просрочено на {-Deadline.Subtract(DateTime.Now)}";
            }
            else
                return $"Осталось {Deadline.Subtract(DateTime.Now)}";
        }

        [DataType(DataType.Date)]
        public DateTime DateLastChange { get; set; }

        public AssignmentVM(Assignment assignment)
        {
            Id = assignment.Id;
            Text = assignment.Text;
            Deadline = assignment.Deadline;
            ExecutorsList = new string("");
            if (assignment.Executors != null)
            {
                foreach (Executor ex in assignment.Executors)
                    ExecutorsList += $"{ex.ThirstName} {ex.SecondName} {ex.ThirdName}, ";
                ExecutorsList = ExecutorsList.Substring(0, ExecutorsList.Length - 2);
            }
            if (assignment.Done)
                Done = "Yes";
            else
                Done = "No";
            DateLastChange = assignment.DateLastChange();
        }
    }

    public class AssignmentExVM
    {
        public long Id { get; set; }
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        public string Done { get; set; }
        public string is_Overdue()
        {
            if (Deadline.Subtract(DateTime.Now).TotalSeconds < 0)
            {
                return $"Просрочено на {-Deadline.Subtract(DateTime.Now)}";
            }
            else if (Done == "No")
                return $"Осталось {Deadline.Subtract(DateTime.Now)}";
            else
                return "";
        }

        public AssignmentExVM(long a_id, string a_text, DateTime a_deadline, bool a_done)
        {
            Id = a_id;
            Text = a_text;
            Deadline = a_deadline;
            if (a_done)
                Done = "Yes";
            else
                Done = "No";
        }
    }
}
