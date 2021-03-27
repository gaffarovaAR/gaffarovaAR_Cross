using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Models
{
    public class Attempt
    {
        public long ID { get; set; }
        public Executor Executor { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateChange { get; set; }
        public string Report { get; set; }
        public string Comment { get; set; }

        public Attempt()
        {
            DateChange = DateTime.Now;
        }
        public Attempt(AttemptDTO attemptDTO, Executor executor)
        {
            DateChange = DateTime.Now;
            Executor = executor;
            Report = attemptDTO.Report;
        }
    }

    public class AttemptDTO
    {
        public long ExecutorID { get; set; }
        public string Report { get; set; }
    }

    public class AttemptVM
    {
        public long ID { get; set; }
        public string Executor{ get; set; }
        public string Report { get; set; }
        public DateTime DateChange { get; set; }
        public string Comment { get; set; }

        public AttemptVM() { }
        public AttemptVM(Attempt attempt)
        {
            ID = attempt.ID;
            Executor = $"{attempt.Executor.ThirstName} {attempt.Executor.SecondName} {attempt.Executor.ThirdName}";
            Report = attempt.Report;
            DateChange = attempt.DateChange;
            Comment = attempt.Comment;
        }
    }
}
