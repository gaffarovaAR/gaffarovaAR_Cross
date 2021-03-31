using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GaffarovaAlbina.Models;
using Microsoft.AspNetCore.Authorization;

namespace GaffarovaAlbina.Controllers
{
    [Route("api/Protocols")]
    [ApiController]
    public class ProtocolsController : ControllerBase
    {
        private readonly DocumentContext _context;
        private readonly IManager _manager;

        public ProtocolsController(DocumentContext context, IManager manager)
        {
            _context = context;
            _manager = manager;
        }

        // GET: api/Protocols
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ProtocolVM>>> GetProtocols()
        {
            var protocols = await _context.Protocols.ToListAsync();
            List<ProtocolVM> protocolVMs = new List<ProtocolVM>();
            foreach (Protocol prot in protocols)
                protocolVMs.Add(new ProtocolVM(prot));
            return protocolVMs;
        }

        // GET: api/Protocols/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProtocolVM>> GetProtocol(long id)
        {
            var protocol = await _context.Protocols.FindAsync(id);

            if (protocol == null)
            {
                return NotFound(new { errorText = $"Protocol with id = {id} was not found." });
            }

            return new ProtocolVM(protocol);
        }

        // PUT: api/Protocols/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProtocolVM>> PutProtocol(long id, ProtocolDTO protocolDTO)
        {
            List<Assignment> assignments = new List<Assignment>();

            foreach (long prID in protocolDTO.assignmentsID)
            {
                var assignment = await _context.Assignments.FindAsync(prID);

                if (assignment == null)
                {
                    return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
                }
                else
                    assignments.Add(assignment);
            }

            Executor head = await _context.Executors.FindAsync(protocolDTO.HeadID);
            if (head == null)
                return NotFound(new { errorText = $"Head with id = {protocolDTO.HeadID} was not found." });

            Protocol protocol = new Protocol(protocolDTO, assignments, head);
            protocol.Id = id;

            _context.Entry(protocol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProtocolExists(id))
                {
                    return NotFound(new { errorText = $"Protocol with id = {id} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new ProtocolVM(protocol);
        }

        // POST: api/Protocols
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProtocolVM>> PostProtocol(ProtocolDTO protocolDTO)
        {
            List<Assignment> assignments = new List<Assignment>();

            foreach (long id in protocolDTO.assignmentsID)
            {
                var assignment = await _context.Assignments.FindAsync(id);

                if (assignment == null)
                {
                    return NotFound(new { errorText = $"Assignment with id = {id} was not found." });
                }
                else
                    assignments.Add(assignment);
            }

            Executor head = await _context.Executors.FindAsync(protocolDTO.HeadID);
            if (head == null)
                return NotFound(new { errorText = $"Head with id = {protocolDTO.HeadID} was not found." });

            Protocol protocol = new Protocol(protocolDTO, assignments, head);

            _context.Protocols.Add(protocol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProtocol), new { id = protocol.Id }, new ProtocolVM(protocol));
        }

        // DELETE: api/Protocols/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProtocol(long id)
        {
            var protocol = await _context.Protocols.FindAsync(id);
            if (protocol == null)
            {
                return NotFound(new { errorText = $"Protocol with id = {id} was not found." });
            }

            _context.Protocols.Remove(protocol);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ProtocolExists(long id)
        {
            return _context.Protocols.Any(e => e.Id == id);
        }

        // PUT: api/Protocols/5/Assignment/5
        [HttpPut("{id_prot}/Assignment/{id_ass}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProtocolVM>> PutProtocolAssig(long id_prot, long id_ass)
        {
            Assignment assignment = await _context.Assignments.FindAsync(id_ass);
            if (assignment == null)
                NotFound(new { errorText = $"Assignment with id = {id_ass} was not found." });

            Protocol protocol = _manager.SetAssignment(await _context.Protocols.FindAsync(id_prot), assignment);
            _context.Entry(protocol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProtocolExists(id_prot))
                {
                    return NotFound(new { errorText = $"Protocol with id = {id_prot} was not found." });
                }
                else
                {
                    throw;
                }
            }

            return new ProtocolVM(protocol);
        }
    }
}
