namespace WebAPI_Log.Models.DTOs
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string? NombreCompleto { get; set; }
        public int IdPerfil { get; set; }
        public string? NombrePerfil { get; set; }
    }
}
