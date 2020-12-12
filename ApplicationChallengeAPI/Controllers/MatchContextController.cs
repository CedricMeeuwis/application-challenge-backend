using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchContextController : ControllerBase
    {
        private readonly TafeltennisContext _context;

        public MatchContextController(TafeltennisContext context)
        {
            _context = context;
        }

        // GET: api/MatchContext
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchContext>>> GetMatchContexten()
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            return await _context.MatchContexten.ToListAsync();
        }
        // GET: api/MatchContext/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchContext>> GetMatchContext(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            var ploeg = await _context.MatchContexten.SingleOrDefaultAsync(i => i.MatchContextID == id);

            if (ploeg == null)
            {
                return NotFound();
            }

            return ploeg;
        }

        // PUT: api/MatchContext/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatchContext(int id, MatchContext ploeg)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

            if (id != ploeg.MatchContextID)
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
                if (!MatchContextExists(id))
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

        // POST: api/MatchContext
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MatchContext>> PostMatchContext(MatchContext ploeg)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

            _context.MatchContexten.Add(ploeg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatchContext", new { id = ploeg.MatchContextID }, ploeg);
        }

        // DELETE: api/MatchContext/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<MatchContext>> DeleteMatchContext(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

            var ploeg = await _context.MatchContexten.FindAsync(id);
            if (ploeg == null)
            {
                return NotFound();
            }

            _context.MatchContexten.Remove(ploeg);
            await _context.SaveChangesAsync();

            return ploeg;
        }

        private bool MatchContextExists(int id)
        {
            return _context.MatchContexten.Any(e => e.MatchContextID == id);
        }
    }
}
