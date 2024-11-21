namespace WebAPI_Log.Entities
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroContacto { get; set; }
        public DateTime FechaIncripcion { get; set; }
        public int IdPerfil { get; set; }
        //Id relacion tabla perfil EF 
        public virtual Perfil PerfilReferencia { get; set; }
        
    }
}
