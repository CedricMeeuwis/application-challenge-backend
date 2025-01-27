﻿using System;
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
    public class TafelController : ControllerBase
    {
        private readonly TafeltennisContext _context;

        public TafelController(TafeltennisContext context)
        {
            _context = context;
        }

        // GET: api/Tafel
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tafel>>> GetTafels()
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            return await _context.Tafels.ToListAsync();
        }

        // GET: api/Tafel/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Tafel>> GetTafel(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            var tafel = await _context.Tafels.FindAsync(id);

            if (tafel == null)
            {
                return NotFound();
            }

            return tafel;
        }

        // PUT: api/Tafel/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTafel(int id, Tafel tafel)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

            if (id != tafel.TafelID)
            {
                return BadRequest();
            }

            _context.Entry(tafel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TafelExists(id))
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

        // POST: api/Tafel
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Tafel>> PostTafel(Tafel tafel)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

            _context.Tafels.Add(tafel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TafelExists(tafel.TafelID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTafel", new { id = tafel.TafelID }, tafel);
        }

        // DELETE: api/Tafel/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tafel>> DeleteTafel(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            var tafel = await _context.Tafels.FindAsync(id);
            if (tafel == null)
            {
                return NotFound();
            }

            _context.Tafels.Remove(tafel);
            await _context.SaveChangesAsync();

            return tafel;
        }
        private bool TafelExists(int id)
        {
            return _context.Tafels.Any(e => e.TafelID == id);
        }
    }
}
