using Microsoft.AspNetCore.Identity;

namespace WebAPI_Log.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? Nombre { get; set; }

        public string? Correo { get; set; }

        public string? Clave { get; set; }

        public List<IdentityUserRole<int>>? UserRoles { get; set; }
    }
}
