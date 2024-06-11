using HomeBankingNet8.DTOs;
using HomeBankingNet8.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HomeBankingNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IClientRepository clientRepository, IConfiguration configuration)
        {
            _clientRepository = clientRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] ClientLoginDTO client) 
        {
            try
            {
                var user = _clientRepository.FindByEmail(client.Email);
                if (user == null || !VerificarPassword(client.Password,user.Password)){
                    return Unauthorized();
                }
                var claims = new List<Claim>();
                if (String.Equals(user.Email, "agusaar@gmail.com"))
                {
                    claims.Add(new Claim("Client", user.Email));
                    claims.Add(new Claim("Admin", user.Email));
                }
                else
                    claims.Add(new Claim("Client", user.Email));

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration["Jwt:SecretKey"]));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var token = new JwtSecurityToken(
                                       claims: claims,
                                       expires: DateTime.Now.AddMinutes(10),
                                       signingCredentials: cred
                );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(jwt);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public bool VerificarPassword(string passwordIngresada, string passwordHasheada)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(passwordIngresada));
                string hash = Convert.ToBase64String(hashBytes);

                return hash == passwordHasheada;
            }
        }

    }
}
