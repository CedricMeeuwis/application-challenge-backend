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
        private readonly ChallengeContext _context;

        public UserController(IUserService userService, ChallengeContext context)
        {
            _userService = userService;
            _context = context;
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //var email = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;

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
        // GET: api/User/Ploeg/1
        [HttpGet("Ploeg/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByPloeg(int id)
        {
            return await _context.Users.Where(x => x.PloegID == id).ToListAsync();
        }
        // GET: api/User/1
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
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

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            if (user.Passwoord != "")
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
            }

            _context.Entry(user).State = EntityState.Modified;

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

            return user;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
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