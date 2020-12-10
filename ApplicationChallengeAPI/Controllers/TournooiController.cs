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
    public class TournooiController : ControllerBase
    {
        private readonly TafeltennisContext _context;

        public TournooiController(TafeltennisContext context)
        {
            _context = context;
        }

        // GET: api/Tournooi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournooi>>> GetTournooien()
        {
            return await _context.Tournooien.ToListAsync();
        }
        // GET: api/Tournooi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournooi>> GetTournooi(int id)
        {
            var tournooi = await _context.Tournooien.SingleOrDefaultAsync(i => i.TournooiID == id);

            if (tournooi == null)
            {
                return NotFound();
            }

            return tournooi;
        }

        // PUT: api/Tournooi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournooi(int id, Tournooi tournooi)
        {
            if (id != tournooi.TournooiID)
            {
                return BadRequest();
            }

            _context.Entry(tournooi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournooiExists(id))
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

        // POST: api/Tournooi
        [HttpPost]
        public async Task<ActionResult<Tournooi>> PostTournooi(Tournooi tournooi)
        {
            _context.Tournooien.Add(tournooi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTournooi", new { id = tournooi.TournooiID }, tournooi);
        }

        // DELETE: api/Tournooi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tournooi>> DeleteTournooi(int id)
        {
            var tournooi = await _context.Tournooien.FindAsync(id);
            if (tournooi == null)
            {
                return NotFound();
            }

            _context.Tournooien.Remove(tournooi);
            await _context.SaveChangesAsync();

            return tournooi;
        }

        private bool TournooiExists(int id)
        {
            return _context.Tournooien.Any(e => e.TournooiID == id);
        }
    }
}
