using System.Collections.Generic;

namespace GaffarovaAlbina.Models
{
    public class Executor : Account
    {
        public string ThirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string Position { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Executor() : base() { }
        public Executor(ExecutorDTO executorDTO)
        {
            ThirstName = executorDTO.ThirstName;
            SecondName = executorDTO.SecondName;
            ThirdName = executorDTO.ThirdName;
            Position = executorDTO.Position;
            Login = executorDTO.Login;
            Password = executorDTO.Password;
            Role = "user";
        }
    }

    public class ExecutorDTO
    {
        public string ThirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string Position { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class ExecutorVM
    {
        public long ID { get; set; }
        public string ThirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string Position { get; set; }
        public int AmountAssig { get; set; }

        public ExecutorVM() { }
        public ExecutorVM(Executor executor)
        {
            ID = executor.ID;
            ThirstName = executor.ThirstName;
            SecondName = executor.SecondName;
            ThirdName = executor.ThirdName;
            Position = executor.Position;
            if (executor.Assignments != null && executor.Assignments.Count != 0 )
                AmountAssig = executor.Assignments.Count;
            else
                AmountAssig = 0;
        }
    }
}
