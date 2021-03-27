using GaffarovaAlbina.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Controllers
{
    [Route("api/Assignments")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly DocumentContext _context;
        private readonly IManager _manager;

        public AssignmentsController(DocumentContext context, IManager manager)
        {
            _context = context;
            _manager = manager;
        }

        // GET: api/Assignments
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<AssignmentVM>>> GetAssignments()
        {
            List<Assignment> assignments = _context.Assignments.ToList();
            if (assignments.Count == 0)
                return NotFound(new { errorText = "No assignment was not found." });
            List<AssignmentVM> assignmentVMs = new List<AssignmentVM>();

            foreach (Assignment ass in assignments)
                assignmentVMs.Add(new AssignmentVM(ass));
            return assignmentVMs;
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<AssignmentVM>> GetAssignment(long id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
            }

            return new AssignmentVM(assignment);
        }

        // PUT: api/Assignments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AssignmentVM>> PutAssignment(long id, AssignmentDTO assignmentDTO)
        {
            List<Executor> executors = new List<Executor>();

            foreach (long assID in assignmentDTO.ExecutorsID)
            {
                var executor = await _context.Executors.FindAsync(assID);

                if (executor == null)
                {
                    return NotFound(new { errorText = $"Executor with id = {assID} was not found." });
                }
                else
                    executors.Add(executor);
            }

            Assignment assignment = new Assignment(assignmentDTO, executors);
            assignment.DateCreate = DateTime.Now;
            assignment.Id = id;

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
                {
                    return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new AssignmentVM(assignment);
        }

        // POST: api/Assignments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AssignmentVM>> PostAssignment(AssignmentDTO assignmentDTO)
        {
            List<Executor> executors = new List<Executor>();

            foreach(long id in assignmentDTO.ExecutorsID)
            {
                var executor = await _context.Executors.FindAsync(id);

                if (executor == null)
                {
                    return NotFound(new { errorText = $"Executor with id = {id} was not found." });
                }
                else
                    executors.Add(executor);
            }

            Assignment assignment = new Assignment(assignmentDTO, executors);
            assignment.DateCreate = DateTime.Now;
            _context.Assignments.Add(assignment);
            //foreach (var ex in executors)
            //{
            //    ex.Assignments.Add(assignment);
            //}

            await _context.SaveChangesAsync();

            AssignmentVM assignmentVM = new AssignmentVM(assignment);
            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignmentVM);
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAssignment(long id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
            }

            _context.Assignments.Remove(assignment);

            if (assignment.Executors != null)
                foreach (Executor ex in assignment.Executors)
                {
                    var executor = await _context.Executors.FindAsync(ex.ID);
                    executor.Assignments.Remove(assignment);
                }

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AssignmentExists(long id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }

        // GET: api/Assignments/Executor/5
        [HttpGet("Executor/{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<List<AssignmentExVM>>> GetExecAssig(long id)
        {
            var executor = await _context.Executors.FindAsync(id);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            List<(long, string, DateTime, bool)> assignments = _manager.FindExecAssig(_context.Assignments.ToList(), executor);
            List<AssignmentExVM> assignmentExVMs = new List<AssignmentExVM>();
            foreach (var ass in assignments)
                assignmentExVMs.Add(new AssignmentExVM(ass.Item1, ass.Item2, ass.Item3, ass.Item4));

            return assignmentExVMs;
        }

        // GET: api/Assignments/Protocol/5
        [HttpGet("Protocol/{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<List<AssignmentVM>>> GetProtAssig(long id)
        {
            var protocol = await _context.Protocols.FindAsync(id);

            if (protocol == null)
            {
                return NotFound(new { errorText = $"Protocol with id = {id} was not found." });
            }

            List<Assignment> assignments = _manager.FindProtAssig(_context.Protocols.ToList(), protocol);
            List<AssignmentVM> assignmentVMs = new List<AssignmentVM>();
            foreach (Assignment ass in assignments)
                assignmentVMs.Add(new AssignmentVM(ass));

            return assignmentVMs;
        }

        // GET: api/Assignments/Overdue
        [HttpGet("Overdue")]
        [Authorize(Roles = "admin")]
        public List<AssignmentVM> GetOverAssig()
        {
            List<AssignmentVM> assignmentVMs = new List<AssignmentVM>();
            List<Assignment> assignments = _manager.FindOverAssig(_context.Assignments.ToList());
            foreach (Assignment ass in assignments)
                assignmentVMs.Add(new AssignmentVM(ass));
            return assignmentVMs;
        }

        // GET: api/Assignments/NoDone
        [HttpGet("NoDone")]
        [Authorize(Roles = "admin")]
        public List<AssignmentVM> GetNoDoneAssig()
        {
            List<AssignmentVM> assignmentVMs = new List<AssignmentVM>();
            List<Assignment> assignments = _manager.FindNoDoneAssig(_context.Assignments.ToList());
            foreach (Assignment ass in assignments)
                assignmentVMs.Add(new AssignmentVM(ass));
            return assignmentVMs;
        }

        // GET: api/Assignments/5/Attempts
        [HttpGet("{id}/Attempts")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<List<AttemptVM>>> GetAssigAtt(long id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
            }

            List<Attempt> attempts = assignment.History;
            List<AttemptVM> attemptVMs = new List<AttemptVM>();
            if (attempts != null)
            {
                foreach (Attempt att in attempts)
                    attemptVMs.Add(new AttemptVM(att));
            }
            return attemptVMs;
        }

        // GET: api/Assignments/AmountDone
        [HttpGet("AmountDone")]
        [Authorize(Roles = "admin")]
        public int GetAmountDoneAssig()
        {
            return _manager.CountDoneAssig(_context.Assignments.ToList());
        }

        // GET: api/Assignments/AmountOver
        [HttpGet("AmountOver")]
        [Authorize(Roles = "admin")]
        public int GetAmountOverAssig()
        {
            return _manager.CountOverAssig(_context.Assignments.ToList());
        }

        // GET: api/Assignments/AmountWait
        [HttpGet("AmountWait")]
        [Authorize(Roles = "admin")]
        public int GetAmountWaitAssig()
        {
            return _manager.CountWaitAssig(_context.Assignments.ToList());
        }

        // GET: api/Assignments/Executor/5/AmountDone
        [HttpGet("Executor/{id}/AmountDone")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<int>> GetAmountExecDoneAssig(long id)
        {
            var executor = await _context.Executors.FindAsync(id);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            return _manager.CountExecDoneAssig(_context.Assignments.ToList(), executor);
        }

        // GET: api/Assignments/Executor/5/AmountOver
        [HttpGet("Executor/{id}/AmountOver")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<int>> GetAmountExecOverAssig(long id)
        {
            var executor = await _context.Executors.FindAsync(id);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            return _manager.CountExecOverAssig(_context.Assignments.ToList(), executor);
        }

        // GET: api/Assignments/Executor/5/AmountWait
        [HttpGet("Executor/{id}/AmountWait")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<int>> GetAmountExecWaitAssig(long id)
        {
            var executor = await _context.Executors.FindAsync(id);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            return _manager.CountExecWaitAssig(_context.Assignments.ToList(), executor);
        }

        // PUT: api/Assignments/5/SetDone
        [HttpPut("{id}/SetDone")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AssignmentVM>> PutDoneAssignment(long id)
        {
            Assignment assignment = _manager.SetDone(await _context.Assignments.FindAsync(id));
            assignment.Done = !assignment.Done;

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
                {
                    return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new AssignmentVM(assignment);
        }

    }
}
