using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonasAPI.Data;
using PersonasAPI.Modelo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PersonasAPI.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _contexto;
        private readonly IConfiguration _configuration;
        public UsuarioRepositorio(ApplicationDbContext contexto, IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }
        public async Task<string> Login(string userName, string password)
        {
            var user = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));
            if(user == null)
            {
                return "nouser";
            }
            else if(!VerificarPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return "wrongpassword";
            }
            else
            {
                return CrearToken(user);
            }

        }

        public async Task<int> Register(Usuario usuario, string password)
        {
            try
            {
                if(await UserExiste(usuario.UserName))
                {
                    return -1;
                }
                CrearPasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                usuario.PasswordHash = passwordHash;
                usuario.PasswordSalt = passwordSalt;

                await _contexto.Usuarios.AddAsync(usuario);
                await _contexto.SaveChangesAsync();
                return usuario.Id;
            }
            catch(Exception e)
            {
                return -500; ;
            }
        }

        public async Task<bool> UserExiste(string userName)
        {
            if(await _contexto.Usuarios.AnyAsync(x => x.UserName.ToLower().Equals(userName)))
            {
                return true;
            }
            return false;
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }            
        }

        private string CrearToken(Usuario user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
