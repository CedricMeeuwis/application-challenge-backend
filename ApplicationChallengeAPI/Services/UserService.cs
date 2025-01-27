﻿using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Helpers;
using ApplicationChallengeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly TafeltennisContext _newsContext;

        public UserService(IOptions<AppSettings> appSettings, TafeltennisContext newsContext)
        {
            _appSettings = appSettings.Value;
            _newsContext = newsContext;
        }

        public User Authenticate(string email, string passwoord)
        {
            var user = _newsContext.Users.Include(p=>p.Ploeg).SingleOrDefault(x => x.Email == email && x.Passwoord == passwoord);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("Naam", user.Naam),
                    new Claim("IsAdmin", user.IsAdmin.ToString()),
                    new Claim("IsKapitein", user.IsKapitein.ToString()),
                    new Claim("PloegID", user.PloegID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Passwoord = null;

            return user;
        }

    }
}
