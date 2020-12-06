using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PloegController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public PloegController(ChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Ploeg
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ploeg>>> GetPloegen()
        {
            return await _context.Ploegen.Include(k => k.Kapitein).ToListAsync();
        }
        // GET: api/Ploeg/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ploeg>> GetPloeg(int id)
        {
            var ploeg = await _context.Ploegen.Include(k => k.Kapitein).SingleOrDefaultAsync(i => i.PloegID == id);

            if (ploeg == null)
            {
                return NotFound();
            }

            return ploeg;
        }

        // PUT: api/Ploeg/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPloeg(int id, Ploeg ploeg)
        {
            if (id != ploeg.PloegID)
            {
                return BadRequest();
            }

            _context.Entry(ploeg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PloegExists(id))
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

        // POST: api/Ploeg
        [HttpPost]
        public async Task<ActionResult<Ploeg>> PostPloeg(Ploeg ploeg)
        {
            _context.Ploegen.Add(ploeg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPloeg", new { id = ploeg.PloegID }, ploeg);
        }

        // DELETE: api/Ploeg/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ploeg>> DeletePloeg(int id)
        {
            var ploeg = await _context.Ploegen.FindAsync(id);
            if (ploeg == null)
            {
                return NotFound();
            }

            _context.Ploegen.Remove(ploeg);
            await _context.SaveChangesAsync();

            return ploeg;
        }

        private bool PloegExists(int id)
        {
            return _context.Ploegen.Any(e => e.PloegID == id);
        }
    }
}
