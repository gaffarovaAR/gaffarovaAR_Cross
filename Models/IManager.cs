using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Models
{
    public interface IManager
    {
        public List<(long, string, DateTime, bool)> FindExecAssig(List<Assignment> assignments, Executor executor);
        public List<Assignment> FindProtAssig(List<Protocol> protocols, Protocol protocol);
        public List<Assignment> FindOverAssig(List<Assignment> assignments);
        public List<Assignment> FindNoDoneAssig(List<Assignment> assignments);
        public int CountDoneAssig(List<Assignment> assignments);
        public int CountOverAssig(List<Assignment> assignments);
        public int CountWaitAssig(List<Assignment> assignments);
        public int CountExecDoneAssig(List<Assignment> assignments, Executor executor);
        public int CountExecOverAssig(List<Assignment> assignments, Executor executor);
        public int CountExecWaitAssig(List<Assignment> assignments, Executor executor);
        public Assignment SetDone(Assignment assignment);
        public Assignment SetAttempt(Assignment assignment, Attempt attempt);
        public Attempt SetComment(Attempt attempt, string comment);
        public Protocol SetAssignment(Protocol protocol, Assignment assignment);
    }
}
