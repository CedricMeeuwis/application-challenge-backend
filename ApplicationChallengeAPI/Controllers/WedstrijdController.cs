﻿using System;
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
    public class WedstrijdController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public WedstrijdController(ChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Wedstrijd
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijden()
        {
            return await _context.Wedstrijden
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .ToListAsync();
        }
        // GET: api/Wedstrijd
        [HttpGet("Tournooi/{id}")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijdenOfTournooi(int id)
        {
            return await _context.Wedstrijden.Where(w => w.MatchContext.TournooiID == id).OrderBy(w => w.MatchContext.TournooiRangschikking)
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext)
                .ToListAsync();
        }
        // GET: api/Wedstrijd/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wedstrijd>> GetWedstrijd(int id)
        {
            var wedstrijd = await _context.Wedstrijden
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .SingleOrDefaultAsync(i => i.WedstrijdID == id);

            if (wedstrijd == null)
            {
                return NotFound();
            }

            return wedstrijd;
        }

        // PUT: api/Wedstrijd/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWedstrijd(int id, Wedstrijd wedstrijd)
        {
            if (id != wedstrijd.WedstrijdID)
            {
                return BadRequest();
            }

            _context.Entry(wedstrijd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WedstrijdExists(id))
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

        // POST: api/Wedstrijd
        [HttpPost]
        public async Task<ActionResult<Wedstrijd>> PostWedstrijd(Wedstrijd wedstrijd)
        {
            _context.Wedstrijden.Add(wedstrijd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWedstrijd", new { id = wedstrijd.WedstrijdID }, wedstrijd);
        }

        // DELETE: api/Wedstrijd/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Wedstrijd>> DeleteWedstrijd(int id)
        {
            var wedstrijd = await _context.Wedstrijden.FindAsync(id);
            if (wedstrijd == null)
            {
                return NotFound();
            }
            var matchContext = await _context.MatchContexten.FindAsync(wedstrijd.MatchContextID);

            _context.Wedstrijden.Remove(wedstrijd);
            _context.MatchContexten.Remove(matchContext);
            await _context.SaveChangesAsync();

            return wedstrijd;
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijdenUser(int id)
        {
            return await _context.Wedstrijden
                .Where(w => (w.Team1User1ID == id ||
                            w.Team1User2ID == id ||
                            w.Team2User1ID == id ||
                            w.Team2User2ID == id) &&
                            w.Bezig == false &&
                            w.Akkoord == true &&
                           (w.Team1Score != 0 && w.Team2Score != 0)
                )
                .Select( w =>
                    new Wedstrijd
                    {
                        WedstrijdID = w.WedstrijdID,
                        MatchContext = new MatchContext
                        {
                            MatchContextID = w.MatchContext.MatchContextID,
                            TournooiID = w.MatchContext.TournooiID,
                            Tournooi = w.MatchContext.Tournooi,
                            TournooiNiveau = w.MatchContext.TournooiNiveau,
                            TournooiRangschikking = w.MatchContext.TournooiRangschikking,
                            CompetitieID = w.MatchContext.CompetitieID,
                            Competitie = w.MatchContext.Competitie
                        },
                        MatchContextID = w.MatchContextID,
                        Tafel = w.Tafel,
                        TafelID = w.TafelID,
                        Team1User1ID = w.Team1User1ID,
                        Team1User1 = new User
                        {
                            UserID = w.Team1User1.UserID,
                            Naam = w.Team1User1.Naam,
                            PloegID = w.Team1User1.PloegID,
                            Ploeg = w.Team1User1.Ploeg
                        },
                        Team1User2ID = w.Team1User2ID,
                        Team1User2 = new User
                        {
                            UserID = w.Team1User2.UserID,
                            Naam = w.Team1User2.Naam
                        },
                        Team2User1ID = w.Team2User1ID,
                        Team2User1 = new User
                        {
                            UserID = w.Team2User1.UserID,
                            Naam = w.Team2User1.Naam,
                            PloegID = w.Team2User1.PloegID,
                            Ploeg = w.Team2User1.Ploeg
                        },
                        Team2User2ID = w.Team2User2ID,
                        Team2User2 = new User
                        {
                            UserID = w.Team2User2.UserID,
                            Naam = w.Team2User2.Naam
                        },
                        Team1Score = w.Team1Score,
                        Team2Score = w.Team2Score
                    }
                )
                .OrderByDescending(w => w.WedstrijdID)
                .ToListAsync();
        }

        private bool WedstrijdExists(int id)
        {
            return _context.Wedstrijden.Any(e => e.WedstrijdID == id);
        }
    }
}
