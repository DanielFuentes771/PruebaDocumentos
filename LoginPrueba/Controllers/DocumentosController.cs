using LoginPrueba.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace LoginPrueba.Controllers
{
    [ApiController]
    [Route("Documentos")]
   // [Authorize]
    public class DocumentosController : Controller
    {
        private readonly LoginPruebasContext _db;
        private readonly IConfiguration _configuracion;
        private readonly ILogger<DocumentosController> _logger;

        public DocumentosController(LoginPruebasContext db, ILogger<DocumentosController> logger, IConfiguration configuracion)
        {
            _logger = logger;
            _db = db;
            _configuracion = configuracion;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("listar")]
        [SwaggerOperation(Summary = "Obtiene la lista de documentos registrados")]
        public async Task<IActionResult> List()
        {
            var documentos = await _db.Documentos
                .Include(d => d.Usuario)
                .Select(d => new
                {
                    d.Id,
                    d.nombre,
                    d.contrato,
                    d.saldo,
                    Fecha =d.fecha.ToString("yyyy-MM-dd"),
                    d.telefono,
                    Usuario = d.Usuario != null ? d.Usuario.usuario : "Sin asignar",
                    FechaCreacion = d.fechaCreacion.HasValue ? d.fechaCreacion.Value.ToString("yyyy-MM-dd HH:mm") : ""
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(documentos);
        }
        [HttpPost("crear")]
        [SwaggerOperation(Summary = "Crea un nuevo documento")]
        public async Task<IActionResult> Create([FromBody] Documento datos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var documento = new Documento
                {
                    nombre = datos.nombre,
                    contrato = datos.contrato,
                    saldo = datos.saldo,
                    fecha = datos.fecha,
                    telefono = datos.telefono,
                    usuarioId = datos.usuarioId,
                    fechaCreacion = DateTime.Now
                };

                _db.Documentos.Add(documento);
                await _db.SaveChangesAsync();

                return new JsonResult(new { id = documento.Id });
            }
            catch (DbUpdateException ex)
            {
                var sqlError = ex.InnerException?.Message;
                _logger.LogError(ex, "Error al crear el documento");
                return StatusCode(500, "Error al guardar el documento: " + sqlError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al crear documento");
                return StatusCode(500, "Error inesperado: " + ex.Message);
            }
        }
        [HttpPost("datatable")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public JsonResult ConsultarDocumentos(
            [FromForm] List<search> search,
            [FromForm] List<Columns> columns,
            [FromForm] List<Order> order,
            [FromForm] int start = 0,
            [FromForm] int length = 10)
        {
            try
            {
                string busqueda = Request.Form["search[value]"];
                var lstDocs = _db.Documentos
                    .Include(d => d.Usuario)
                    .Select(d => new
                    {
                        d.Id,
                        d.nombre,
                        d.contrato,
                        d.saldo,
                        Fecha = d.fecha.ToString("yyyy-MM-dd"),
                        d.telefono,
                        Usuario = d.Usuario != null ? d.Usuario.usuario : "Sin usuario",
                        d.fechaCreacion
                    })
                    .Cast<object>()
                    .ToList();

                return new JsonResult(new Tablex().Tables(busqueda, search, columns, order, start, length, lstDocs).Value);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError(ex, "Error al consultar documentos");
                return new JsonResult("Error al consultar los documentos: " + ex.Message);
            }
        }
        [HttpPut("actualizar/{id:int}")]
        [SwaggerOperation(Summary = "Actualiza los datos de un documento existente")]
        public async Task<IActionResult> Update(int id, [FromBody] Documento datos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existente = await _db.Documentos.FindAsync(id);
                if (existente == null) return NotFound("Documento no encontrado");

                existente.nombre = datos.nombre;
                existente.contrato = datos.contrato;
                existente.saldo = datos.saldo;
                existente.fecha = datos.fecha;
                existente.telefono = datos.telefono;
                existente.usuarioId = datos.usuarioId;

                await _db.SaveChangesAsync();

                return new JsonResult(new { id = existente.Id });
            }
            catch (SqlException ex)
            {
                var sqlError = ex.InnerException?.Message;
                _logger.LogError(ex, "Error al actualizar documento");
                return StatusCode(500, "Error al actualizar el documento: " + sqlError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar documento");
                return StatusCode(500, "Error inesperado: " + ex.Message);
            }
        }
        [HttpDelete("eliminar/{id:int}")]
        [SwaggerOperation(Summary = "Elimina un documento por su ID")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existente = await _db.Documentos.FindAsync(id);
                if (existente == null) return NotFound("Documento no encontrado");

                _db.Documentos.Remove(existente);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                var sqlError = ex.InnerException?.Message;
                _logger.LogError(ex, "Error al eliminar documento");
                return StatusCode(500, "Error al eliminar el documento: " + sqlError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar documento");
                return StatusCode(500, "Error inesperado: " + ex.Message);
            }
        }
        [HttpGet("buscar-usuarios")]
        [SwaggerOperation(Summary = "Busca usuarios para asignar a documentos (usado en Select2)")]
        public IActionResult BuscarUsuarios(string term)
        {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(term))
                query = query.Where(u => u.usuario != null && u.usuario.Contains(term));

            var resultados = query
                .Select(u => new { id = u.id, text = u.usuario })
                .ToList();

            return Json(resultados);
        }
    }
}