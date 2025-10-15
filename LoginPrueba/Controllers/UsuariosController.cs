using LoginPrueba.Models;
using LoginPrueba.Controllers;
using LoginPrueba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace LoginPrueba.Controllers
{
    [ApiController]
    [Route("Usuarios")]
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly LoginPruebasContext _db;
        private readonly IConfiguration _configuracion;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(LoginPruebasContext db, ILogger<UsuariosController> logger, IConfiguration configuracion)
        {
            _logger = logger;
            _db = db;
            _configuracion = configuracion;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Se utiliza para mostrar todas las Citas")]
        public async Task<IActionResult> List()
        {
            var usuarios = await _db.Usuarios.AsNoTracking().ToListAsync();
            return Ok(usuarios);
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Crea una cita con validación de slot")]
        public async Task<IActionResult> CreateUsuario([FromBody] Usuarios datos)
        {
          
            try
            {
                var usuarios = new Usuarios
                {
                    id = datos.id,
                    usuario = datos.usuario,
                    horainicio = datos.horainicio,
                    horafin = datos.horafin,
                    estatus = datos.estatus,
                    contraseña = datos.contraseña
                };

                _db.Usuarios.Add(usuarios);
                await _db.SaveChangesAsync();
                // return CreatedAtAction(nameof(Get), new { id = cita.Id }, cita);
                //  return new JsonResult( cita.Id);
                return new JsonResult(new
                {
                    id = usuarios.id,
                });
            }

            catch (DbUpdateException ex)
            {
                var sqlError = ex.InnerException.Message;
                if (sqlError?.Contains("UQ_Citas_Slot") == true)
                {

                    return Conflict("El cliente ya tiene una cita en ese horario.");
                }
                return StatusCode(500, "Error al guardar la cita: " + sqlError);
            }
        }
        [HttpPost("Lista")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public JsonResult ConsultarCitas(
            [FromForm] List<search> search,
            [FromForm] List<Columns> columns,
            [FromForm] List<Order> order,
            [FromForm] int start = 0,
            [FromForm] int length = 10)
        {
            try
            {
                string busqueda = Request.Form["search[value]"];
                var lstOrdenes = _db.Usuarios
                    .Select(usuario => new
                    {
                        usuario.id,
                        usuario.usuario,
                        usuario.estatus,
                        usuario.horainicio,
                        usuario.horafin
                    }).Cast<object>().ToList();

                return new JsonResult(new Tablex().Tables(busqueda, search, columns, order, start, length, lstOrdenes).Value);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult("Error al consultar las ordenes: " + ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Se utiliza para eliminar un Usuario")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _db.Usuarios.FindAsync(id);
                if (usuario == null) return NotFound();

                var permisos = _db.UsuarioPermisos.Where(up => up.UsuarioId == id).ToList();
                _db.UsuarioPermisos.RemoveRange(permisos);

                _db.Usuarios.Remove(usuario);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                var sqlError = ex.InnerException.Message;

                return StatusCode(500, "Error al guardar la cita: " + sqlError);
            }

        }
        [HttpPut("{id:int}")]

        [SwaggerOperation(Summary = "Actualiza una cita con validación de slot")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuarios datos)
        {

            try
            { 
                var usuarioExistente = await _db.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    return NotFound();
                }
                
                usuarioExistente.usuario = datos.usuario;
                usuarioExistente.horafin = datos.horafin;
                usuarioExistente.horainicio = datos.horainicio;
                usuarioExistente.estatus = datos.estatus;
                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    id = usuarioExistente.id
                });

            }


            catch (SqlException ex)
            {
                var sqlError = ex.InnerException.Message;
                if (sqlError?.Contains("UQ_Citas_Slot") == true)
                {

                    return Conflict("El cliente ya tiene una cita en ese horario.");
                }
                return StatusCode(500, "Error al guardar la cita: " + sqlError);
            }
        }
      
    }
}