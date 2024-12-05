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
        public Utilidades(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public string EncriptarSHA256(string texto)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //Computar el Hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                //Convertir el array de Bits a String 
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) 
                {
                    builder.Append(bytes[i].ToString("X2"));
                }

                return builder.ToString();
            }
        }

        public string generarJWT(Usuario modelo)
        {
            // Crear la informacion del usuario para el token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, modelo.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, modelo.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name, modelo.Nombre ?? string.Empty) 
            };

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
            var jwlConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwlConfig);
        }

        // Metodo para Validacion del token
        public bool validarToken(string token)
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
            catch (Exception ex)
            {
                // Registrar el error (si es necesario)
                // Log.Error("Error al validar el token: " + ex.Message);
                return false;
            }
        }
    }
}
