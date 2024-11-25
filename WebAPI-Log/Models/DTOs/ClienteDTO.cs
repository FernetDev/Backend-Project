namespace WebAPI_Log.Models.DTOs
{
    public class ClienteDTO
    {
        //Datos Cliente
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int ContactNro { get; set; }
        public int IdPerfil { get; set; }
        public string? NombrePerfil { get; set; }

        //Datos Mensualidad
        public DateTime? FechaIngreso { get; set; }
        public bool EstaPagada { get; set; } 
        public DateTime? FechaPago { get; set; } 
        public DateTime? FechaVencimiento { get; set; }
    }
}
