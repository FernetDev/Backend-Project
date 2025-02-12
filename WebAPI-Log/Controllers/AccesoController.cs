using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Log.Custom;
using WebAPI_Log.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using WebAPI_Log.Context;
using WebAPI_Log.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI_Log.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly AppDbContext _appDbContext; // Cambie a AppDbContext
        private readonly Utilidades _utilidades;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccesoController(AppDbContext appDbContext, Utilidades utilidades, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) // Cambie a AppDbContext
        {
            _appDbContext = appDbContext; // Cambie a AppDbContext
            _utilidades = utilidades;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //[Authorize(Roles = "ADMIN, MANAGER")]
        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new ApplicationUser  // Cambie a ApplicationUser
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.EncriptarSHA256(objeto.Clave),
            };

            //await _appDbContext.Usuarios.AddAsync(modeloUsuario);  // Cambie a AppDbContext
            //await _appDbContext.SaveChangesAsync();  // Cambie a AppDbContext
            var userResult = await _userManager.CreateAsync(modeloUsuario, objeto.Clave);
            if (!userResult.Succeeded) return BadRequest(userResult.Errors);

            // Asignar el rol de MANAGER al usuario, si es que register es un endpoint sólo para managers
            var roleResult = await _userManager.AddToRoleAsync(modeloUsuario, "MANAGER");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            if (userResult.Succeeded && roleResult.Succeeded)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        //[Authorize(Roles = "ADMIN, MANAGER, COACH")]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _userManager.FindByEmailAsync(objeto.Correo);
            if (usuarioEncontrado == null) return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "Usuario o contraseña incorrectos" });
            
            var result = await _signInManager.CheckPasswordSignInAsync(usuarioEncontrado, objeto.Clave, false);
            if (!result.Succeeded) return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "Usuario o contraseña incorrectos" });

            //ToBeRemoved
            //var usuarioEncontrado = await _appDbContext.Usuarios  // Cambie a AppDbContext
            //                        .Where(u =>
            //                            u.Correo == objeto.Correo &&
            //                            u.Clave == _utilidades.EncriptarSHA256(objeto.Clave)
            //                        ).FirstOrDefaultAsync();  // Cambie a AppDbContext

            //if (usuarioEncontrado == null)
            //{
                //return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "Usuario o contraseña incorrectos" });
            //}
            //else
            //{
                // Generar el token
                var token = await _utilidades.generarJWT(usuarioEncontrado);

                // Devolver el token junto con la información del usuario
                return StatusCode(StatusCodes.Status200OK, new
                {
                    isSuccess = true,
                    token, //Cambie a simplemente token, porque el nombre de la variable es inferido
                    user = new
                    {
                        id = usuarioEncontrado.Id,
                        name = usuarioEncontrado.Nombre,
                        email = usuarioEncontrado.Correo
                    }
                });
            //}
        }

        // Validación del Token
        [HttpGet]
        [Route("ValidarToken")]
        public IActionResult ValidarToken([FromQuery] string token)
        {
            bool respuesta = _utilidades.ValidarToken(token);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }

    }
}
