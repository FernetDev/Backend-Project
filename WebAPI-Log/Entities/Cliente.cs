namespace WebAPI_Log.Entities
{
    public class Cliente
    {
        // Datos del Cliente
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int ContactNro { get; set; }
        public int IdPerfil { get; set; }

        //Datos de Mensualidad 
        public DateTime? FechaIngreso { get; set; }
        public bool EstaPagada { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        //Id relacion tabla perfil EF 
        public virtual Perfil PerfilReferencia { get; set; }
        
    }
}
