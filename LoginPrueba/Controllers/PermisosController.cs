using LoginPrueba.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginPrueba.Controllers
{
    [Route("Permisos")]
    public class PermisosController : Controller
    {
        private readonly LoginPruebasContext _db;

        public PermisosController(LoginPruebasContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("Lista")]
       
        public IActionResult Lista()
        {
            var permisos = _db.Permisos
                .Select(p => new { p.Id, p.NombreModulo })
                .ToList();

            return Json(new { data = permisos });
        }

        [HttpPost("AsignarUsuario")]
        public IActionResult AsignarUsuario([FromBody] UsuarioPermiso modelo)
        {
            _db.UsuarioPermisos.Add(modelo);
            _db.SaveChanges();
            return Ok();
        }

       
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Permiso permiso)
        {
            _db.Permisos.Add(permiso);
            _db.SaveChanges();
            return Ok();
        }
        
        [HttpGet("BuscarModulos")]
        public IActionResult BuscarModulos(string term)
         {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(p => p.usuario.Contains(term));
            }
            var resultados = query
                .Select(p => new
                {
                    id = p.id,
                    text = p.usuario
                })
                .ToList();

            return Json(resultados);
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Permiso permiso)
        {
            var existente = _db.Permisos.Find(id);
            if (existente == null) return NotFound();

            existente.NombreModulo = permiso.NombreModulo;
            _db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var permiso = _db.Permisos.Find(id);
            if (permiso == null) return NotFound();
            var referencias = _db.UsuarioPermisos.Where(up => up.PermisoId == id).ToList();
            _db.UsuarioPermisos.RemoveRange(referencias);

            _db.Permisos.Remove(permiso);
            _db.SaveChanges();

            return Ok();


        }
    }
}
