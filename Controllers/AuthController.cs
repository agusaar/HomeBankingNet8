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
        private IClientRepository _clientRepository;

        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO client) 
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
                    "ContraseñaAgustinRojasBootcampDotNetMindHub2024MarianitoMarianitoMarianito"));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(
                                       claims: claims,
                                       expires: DateTime.Now.AddMinutes(10),
                                       signingCredentials: cred
                );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                
                /*var claimsIdentity = new ClaimsIdentity(
                    claims,
                    JwtBearerDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    JwtBearerDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));*/

                return Ok(jwt);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
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
