using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Models
{
    public class Manager : IManager
    {
        public List<(long, string, DateTime, bool)> FindExecAssig(List<Assignment> assignments, Executor executor)
        {
            return assignments.Where(ass =>  ass.Executors.Contains(executor) )
                .Select(ass => (ass.Id, ass.Text, ass.Deadline, ass.Done))
                .OrderBy(ass => ass.Done)
                .ToList();
        }
        public List<Assignment> FindProtAssig(List<Protocol> protocols, Protocol protocol)
        {
            return protocols.FirstOrDefault(prot => prot == protocol)?.Assignments;
        }
        public List<Assignment> FindOverAssig(List<Assignment> assignments)
        {
            return assignments.Where(ass => (ass.is_Overdue == true && ass.Done == false))
                .OrderBy(ass => ass.RemainingDays).
                ToList();
        }
        public List<Assignment> FindNoDoneAssig(List<Assignment> assignments)
        {
            return assignments.Where(ass => ass.Done == false).ToList();
        }
        public int CountDoneAssig(List<Assignment> assignments)
        {
            return assignments.Where(ass => ass.Done == true).Count();
        }
        public int CountOverAssig(List<Assignment> assignments)
        {
            return assignments.Where(ass => (ass.Done == false && ass.is_Overdue == true)).Count();
        }
        public int CountWaitAssig(List<Assignment> assignments)
        {
            return assignments.Where(ass => (ass.Done == false && ass.is_Overdue == false)).Count();
        }
        public int CountExecDoneAssig(List<Assignment> assignments, Executor executor)
        {
            return assignments.Where(ass => ass.Executors.Contains(executor))
                .Where(ass => ass.Done == true).Count();
        }
        public int CountExecOverAssig(List<Assignment> assignments, Executor executor)
        {
            return assignments.Where(ass => ass.Executors.Contains(executor))
                .Where(ass => (ass.Done == false && ass.is_Overdue == true)).Count();
        }
        public int CountExecWaitAssig(List<Assignment> assignments, Executor executor)
        {
            return assignments.Where(ass => ass.Executors.Contains(executor))
                .Where(ass => (ass.Done == false && ass.is_Overdue == false)).Count();
        }
        public Assignment SetDone(Assignment assignment)
        {
            assignment.Done = !assignment.Done;
            return assignment;
        }
        public Assignment SetAttempt(Assignment assignment, Attempt attempt)
        {
            if (assignment.History == null)
                assignment.History = new List<Attempt>();
            assignment.History.Add(attempt);
            return assignment;
        }
        public Attempt SetComment(Attempt attempt, string comment)
        {
            attempt.Comment = comment;
            return attempt;
        }
        public Protocol SetAssignment(Protocol protocol, Assignment assignment)
        {
            protocol.Assignments.Add(assignment);
            return protocol;
        }
    }
}
