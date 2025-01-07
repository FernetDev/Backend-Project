using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI_Log.Entities;

namespace WebAPI_Log.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public Utilidades(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager;
        }

        // Metodo para encriptar el texto con SHA-256
        public string EncriptarSHA256(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                throw new ArgumentException("El texto no puede ser nulo o vacío.", nameof(texto));
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                //Computar el Hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                //Convertir el array de Bits a String 
                StringBuilder builder = new StringBuilder();
                foreach (byte byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("X2"));
                }

                return builder.ToString();
            }
        }

        // Metodo para generar el JWT
        public async Task<string> generarJWT(ApplicationUser modelo)
        {
            if (modelo == null)
            {
                throw new ArgumentNullException(nameof(modelo), "El modelo de usuario no puede ser nulo.");
            }

            // Crear la informacion del usuario para el token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, modelo.Id.ToString()),
                new Claim(ClaimTypes.Email, modelo.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name, modelo.Nombre ?? string.Empty)
            };

            //Get roles
            var roles = await _userManager.GetRolesAsync(modelo);

            //Add roles to claims
            var userAndRolesClaims = userClaims.Union(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("La clave de JWT no está configurada.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"] ?? "30");
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            // Crear detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims: userAndRolesClaims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwtConfig);
        }

        // Metodo para Validacion del token
        public bool ValidarToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false; // Retorna false si el token esta vacio o es nulo
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
            };

            try
            {
                // Intentar validar el token
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Si la validación es exitosa, retorna true
                return validatedToken != null;
            }
            catch (Exception)
            {
                // Registrar el error (si es necesario)
                // Log.Error("Error al validar el token.");
                return false;
            }
        }
    }
}
