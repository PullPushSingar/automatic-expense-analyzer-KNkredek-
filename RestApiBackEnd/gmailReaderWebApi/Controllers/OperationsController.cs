
using gmailReaderWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gmailReaderWebApi.Controllers
{
    [Route("Controllers")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Operations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperations()
        {
            return await _context.Operations.ToListAsync();
        }

        // GET: api/Operations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Operation>> GetOperation(int id)
        {
            var operation = await _context.Operations.FindAsync(id);

            if (operation == null)
            {
                return NotFound();
            }

            return operation;
        }
        /*
        // POST: api/Operations
        [HttpPost]
        public async Task<ActionResult<Operation>> PostOperation(Operation operation)
        {
            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { id = operation.OperationId }, operation);
        }
        */

        /*
        // PUT: api/Operations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperation(int id, Operation operation)
        {
            if (id != operation.OperationId)
            {
                return BadRequest();
            }

            _context.Entry(operation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */
        /*
        // DELETE: api/Operations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperation(int id)
        {
            var operation = await _context.Operations.FindAsync(id);
            if (operation == null)
            {
                return NotFound();
            }

            _context.Operations.Remove(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool OperationExists(int id)
        {
            return _context.Operations.Any(e => e.OperationId == id);
        }
    }
}
