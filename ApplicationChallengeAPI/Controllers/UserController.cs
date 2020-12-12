using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;
using ApplicationChallengeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly TafeltennisContext _context;

        public UserController(IUserService userService, TafeltennisContext context)
        {
            _userService = userService;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {

            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!isAdmin)
            {
                return Unauthorized();
            }

            //alle users zonder passwoorden
            return await _context.Users.Select(u => new User
            {
                UserID = u.UserID,
                Naam = u.Naam,
                Email = u.Email,
                Foto = u.Foto,
                Geboortedatum = u.Geboortedatum,
                IsAdmin = u.IsAdmin,
                IsKapitein = u.IsKapitein,
                PloegID = u.PloegID,
                Ploeg = u.Ploeg
            }).ToListAsync();
        }

        [Authorize]
        [HttpGet("Ploegloos")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersZonderPloeg()
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!isAdmin)
            {
                return Unauthorized();
            }

            return await _context.Users.Select(u => new User
            {
                UserID = u.UserID,
                Naam = u.Naam,
                Email = u.Email,
                Foto = u.Foto,
                Geboortedatum = u.Geboortedatum,
                IsAdmin = u.IsAdmin,
                IsKapitein = u.IsKapitein,
                PloegID = u.PloegID,
                Ploeg = u.Ploeg
            }).Where(u => u.PloegID.HasValue == false).ToListAsync();
        }

        // GET: api/User/Ploeg/1
        [Authorize]
        [HttpGet("Ploeg/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByPloeg(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!isAdmin)
            {
                return Unauthorized();
            }

            return await _context.Users.Where(x => x.PloegID == id).ToListAsync();
        }

        [Authorize]
        // GET: api/User/Ploeg
        [HttpGet("Ploeg")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersHasPloeg()
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!isAdmin)
            {
                return Unauthorized();
            }

            return await _context.Users.Where(x => x.PloegID != null).Include(p => p.Ploeg).Select(u => new User
            {
                UserID = u.UserID,
                Naam = u.Naam,
                Email = u.Email,
                Foto = u.Foto,
                Geboortedatum = u.Geboortedatum,
                IsAdmin = u.IsAdmin,
                IsKapitein = u.IsKapitein,
                PloegID = u.PloegID,
                Ploeg = u.Ploeg
            }).ToListAsync();
        }

        // GET: api/User/MijnPloeg
        [Authorize]
        [HttpGet("MijnPloeg")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersFromPloeg()
        {
            if (string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value))
            {
                return NotFound();
            }
            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);

            var users = await _context.Users.Where(u => u.PloegID == ploegID).Select(u => new User
            {
                UserID = u.UserID,
                Naam = u.Naam,
                Email = u.Email,
                IsKapitein = u.IsKapitein,
                PloegID = u.PloegID
            }).ToListAsync();

            return users;
        }

        [Authorize]
        // GET: api/User/1
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!isAdmin)
            {
                return Unauthorized();
            }
            
            var user = await _context.Users.Include(r => r.Ploeg).SingleOrDefaultAsync(i => i.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(user.Passwoord, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            user.Passwoord = savedPasswordHash;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(int id, User user)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (id != user.UserID || !isAdmin)
            {
                return Unauthorized();
            }

            if (user.Passwoord != "" && user.Passwoord != null)
            {
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                var pbkdf2 = new Rfc2898DeriveBytes(user.Passwoord, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);

                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                string savedPasswordHash = Convert.ToBase64String(hashBytes);
                user.Passwoord = savedPasswordHash;

                _context.Entry(user).State = EntityState.Modified;
            }
            else
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.Entry(user).Property(u => u.Passwoord).IsModified = false;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            user.Passwoord = null;
            return user;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User userParam)
        {
            //getuser
            User user = _context.Users.Where(x => x.Email == userParam.Email).FirstOrDefault();
            /* Fetch the stored value */
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            string savedPasswordHash = user.Passwoord;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(userParam.Passwoord, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return Unauthorized();

            var userAuthenticate = _userService.Authenticate(userParam.Email, savedPasswordHash);
            if (userAuthenticate == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (id != user.UserID || !isAdmin)
            {
                return Unauthorized();
            }

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}