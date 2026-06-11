using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EscuelaApp.Services;

namespace EscuelaApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoService _mongo;

        public HomeController(MongoService mongo) => _mongo = mongo;

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var alumnos       = await _mongo.ObtenerAlumnosAsync();
            var calificaciones = await _mongo.ObtenerCalificacionesAsync();

            ViewBag.TotalAlumnos        = alumnos.Count;
            ViewBag.TotalCalificaciones = calificaciones.Count;
            ViewBag.PromedioGeneral     = calificaciones.Count > 0
                ? Math.Round(calificaciones.Average(c => c.Calific), 2)
                : 0;

            return View();
        }
    }
}
