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
    public class PerfilController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PerfilController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<List<PerfilDTO>>> Get()
        {
            var listaDTO = new List<PerfilDTO>();

            foreach (var item in await _context.Perfiles.ToListAsync()){
                listaDTO.Add(new PerfilDTO
                {
                    IdPerfil = item.IdPerfil,
                    Nombre = item.Nombre,
                });
            }
            return Ok(listaDTO);
        }

    }
}
