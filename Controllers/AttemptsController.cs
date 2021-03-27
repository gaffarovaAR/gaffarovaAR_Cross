using GaffarovaAlbina.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Controllers
{
    [Route("api/Attempts")]
    [ApiController]
    public class AttemptsController : ControllerBase
    {
        private readonly DocumentContext _context;
        private readonly IManager _manager;

        public AttemptsController(DocumentContext context, IManager manager)
        {
            _context = context;
            _manager = manager;
        }

        // GET: api/Attempts
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<AttemptVM>>> GetAttempts()
        {
            List<Attempt> attempts = await _context.Attempts.ToListAsync();
            List<AttemptVM> attemptVMs = new List<AttemptVM>();
            foreach (Attempt att in attempts)
                attemptVMs.Add(new AttemptVM(att));
            return attemptVMs;
        }

        // GET: api/Attempts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<AttemptVM>> GetAttempt(long id)
        {
            var attempt = await _context.Attempts.FindAsync(id);

            if (attempt == null)
            {
                return NotFound(new { errorText = $"Attempt with id = {id} was not found." });
            }

            return new AttemptVM(attempt);
        }

        // PUT: api/Attempts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<AttemptVM>> PutAttempt(long id, AttemptDTO attemptDTO)
        {
            var executor = await _context.Executors.FindAsync(attemptDTO.ExecutorID);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            Attempt attempt = new Attempt(attemptDTO, executor);
            attempt.ID = id;

            _context.Entry(attempt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttemptExists(id))
                {
                    return NotFound(new { errorText = $"Attempt with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new AttemptVM(attempt);
        }

        // POST: api/Attempts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<Attempt>> PostAttempt(long id, AttemptDTO attemptDTO)
        {
            var executor = await _context.Executors.FindAsync(attemptDTO.ExecutorID);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }
            else
            {
                Attempt attempt = new Attempt(attemptDTO, executor);
                _context.Attempts.Add(attempt);
                await _context.SaveChangesAsync();

                Assignment assignment = _manager.SetAttempt(await _context.Assignments.FindAsync(id), attempt);

                _context.Entry(assignment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAttempt), new { id = attempt.ID }, new AttemptVM(attempt));
            }
        }

        // DELETE: api/Attempts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAttempt(long id)
        {
            var attempt = await _context.Attempts.FindAsync(id);
            if (attempt == null)
            {
                return NotFound(new { errorText = $"Attempt with id = {id} was not found." });
            }

            _context.Attempts.Remove(attempt);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AttemptExists(long id)
        {
            return _context.Attempts.Any(e => e.ID == id);
        }

        // PUT: api/Attempts/5/SetComment
        [HttpPut("{id}/SetComment")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AttemptVM>> PutAttemptComment(long id, string comment)
        {
            Attempt attempt = _manager.SetComment(await _context.Attempts.FindAsync(id), comment);

            _context.Entry(attempt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttemptExists(id))
                {
                    return NotFound(new { errorText = $"Attempt with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new AttemptVM(attempt);
        }
        private bool AssignmentExists(long id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
