using LoginPrueba.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginPrueba.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginPruebasContext _db;
        private readonly IConfiguration _configuracion;
        private readonly ILogger<LoginController> _logger;
        public LoginController(LoginPruebasContext db, ILogger<LoginController> logger, IConfiguration configuracion)
        {
            _logger = logger;
            _db = db;
            _configuracion = configuracion;
        }
        [HttpPost]
        public IActionResult Autenticar(string usuario, string contraseña)
        {
            var user = _db.Usuarios
                .FirstOrDefault(u => u.usuario == usuario && u.contraseña == contraseña);

            if (user == null)
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View("Index");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuracion["JWT_Configuracion:LLaveSecreta"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.Name, user.usuario)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            HttpContext.Session.SetString("Usuario", user.usuario);
            HttpContext.Session.SetString("TokenJWT", tokenHandler.WriteToken(token));

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") != null)
            {

                return RedirectToAction("Index", "Admin");
            }

            return View(); 
        }

        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
