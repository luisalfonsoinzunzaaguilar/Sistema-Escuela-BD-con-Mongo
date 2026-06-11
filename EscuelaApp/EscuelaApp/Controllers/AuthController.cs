using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using EscuelaApp.Models;
using EscuelaApp.Services;

namespace EscuelaApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly MongoService _mongo;

        public AuthController(MongoService mongo) => _mongo = mongo;

        // GET /Auth/Login
        public IActionResult Login() =>
            User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

        // POST /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _mongo.ObtenerUsuarioPorEmailAsync(model.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError("", "Email o contraseña incorrectos");
                return View(model);
            }

            // Crear sesión con cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id!)
            };

            var identity  = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);
            return RedirectToAction("Index", "Home");
        }

        // GET /Auth/Register
        public IActionResult Register() => View();

        // POST /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _mongo.ExisteUsuarioAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Ese email ya está registrado");
                return View(model);
            }

            await _mongo.CrearUsuarioAsync(new Usuario
            {
                Email        = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });

            return RedirectToAction("Login");
        }

        // POST /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
