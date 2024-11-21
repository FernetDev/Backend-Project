using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using WebAPI_Log.Context;
using WebAPI_Log.Entities;
using WebAPI_Log.Models.DTOs;


namespace WebAPI_Log.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<List<ClienteDTO>>> Get()
        {

            var listaDTO = new List<ClienteDTO>();
            var listaDB = await _context.Clientes.Include(p => p.PerfilReferencia).ToListAsync();

            foreach (var item in listaDB)
            {

                listaDTO.Add(new ClienteDTO
                {
                    IdCliente = item.IdCliente,
                    NombreCompleto = item.NombreCompleto,
                    IdPerfil = item.IdPerfil,
                    NombrePerfil = item.PerfilReferencia.Nombre
                });
            }

            return Ok(listaDTO);
        }

        [HttpGet]
        [Route("buscar/{id}")]
        public async Task<ActionResult<ClienteDTO>> Get(int id)
        {
            var clienteDTO = new ClienteDTO();
            var clienteDB = await _context.Clientes.Include(p => p.PerfilReferencia)
                .Where(e => e.IdCliente == id).FirstAsync();

            clienteDTO.IdCliente = id;
            clienteDTO.NombreCompleto = clienteDB.NombreCompleto;
            clienteDTO.IdPerfil = clienteDB.IdPerfil;
            clienteDTO.NombrePerfil = clienteDB.PerfilReferencia.Nombre;
            return Ok(clienteDTO);
        }

        [HttpPost]
        [Route("guardar")]
        public async Task<ActionResult<ClienteDTO>> Guardar(ClienteDTO clienteDTO)
        {
            var clienteDB = new Cliente
            {
                NombreCompleto = clienteDTO.NombreCompleto,
                NumeroContacto = clienteDTO.NumeroContacto,
                FechaIngreso = clienteDTO.FechaIngreso,
                IdPerfil = clienteDTO.IdPerfil,  
            };

            
            await _context.Clientes.AddAsync(clienteDB);
            await _context.SaveChangesAsync();

      
            return Ok("Cliente agregado");
        }

        [HttpPut]
        [Route("editar")]
        public async Task<ActionResult<ClienteDTO>> Editar(ClienteDTO clienteDTO)
        {
            var clienteDB = await _context.Clientes.Include(p => p.PerfilReferencia)
                .Where(e => e.IdCliente == clienteDTO.IdCliente).FirstOrDefaultAsync();  

            if (clienteDB == null)
            {
                return NotFound("Cliente no encontrado");  
            }

      
            clienteDB.NombreCompleto = clienteDTO.NombreCompleto;
            clienteDB.IdPerfil = clienteDTO.IdPerfil;

           
            _context.Clientes.Update(clienteDB);
            await _context.SaveChangesAsync();

            return Ok("Cliente modificado");
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
          
            var clienteDB = await _context.Clientes.FindAsync(id);  

            if (clienteDB == null)
            {
                return NotFound("Cliente no encontrado"); 
            }


            _context.Clientes.Remove(clienteDB);
            await _context.SaveChangesAsync();

            return Ok("Cliente eliminado");
        }


    }
}
