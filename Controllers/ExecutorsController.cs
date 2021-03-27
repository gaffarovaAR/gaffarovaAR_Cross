using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GaffarovaAlbina.Models;
using Microsoft.AspNetCore.Authorization;

namespace GaffarovaAlbina.Controllers
{
    [Route("api/Executors")]
    [ApiController]
    public class ExecutorsController : ControllerBase
    {
        private readonly DocumentContext _context;

        public ExecutorsController(DocumentContext context)
        {
            _context = context;
        }

        // GET: api/Executors
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ExecutorVM>>> GetExecutor()
        {
            List<Executor> executors = await _context.Executors.ToListAsync();
            List<ExecutorVM> executorVMs = new List<ExecutorVM>();
            foreach (Executor ex in executors)
                executorVMs.Add(new ExecutorVM(ex));
            return executorVMs;
        }

        // GET: api/Executors/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ExecutorVM>> GetExecutor(long id)
        {
            var executor = await _context.Executors.FindAsync(id);

            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            return new ExecutorVM(executor);
        }

        // PUT: api/Executors/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ExecutorVM>> PutExecutor(long id, ExecutorDTO executorDTO)
        {
            Executor executor = new Executor(executorDTO);
            executor.ID = id;

            _context.Entry(executor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExecutorExists(id))
                {
                    return NotFound(new { errorText = $"Executor with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new ExecutorVM(executor);
        }

        // POST: api/Executors
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ExecutorVM>> PostExecutor(ExecutorDTO executorDTO)
        {
            Executor executor = new Executor(executorDTO);
            _context.Executors.Add(executor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExecutor), new { id = executor.ID }, new ExecutorVM(executor));
        }

        // DELETE: api/Executors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteExecutor(long id)
        {
            var executor = await _context.Executors.FindAsync(id);
            if (executor == null)
            {
                return NotFound(new { errorText = $"Executor with id = {id} was not found." });
            }

            _context.Executors.Remove(executor);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ExecutorExists(long id)
        {
            return _context.Executors.Any(e => e.ID == id);
        }
    }
}
