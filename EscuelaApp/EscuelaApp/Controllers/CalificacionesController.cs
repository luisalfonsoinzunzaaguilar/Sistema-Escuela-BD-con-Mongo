using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EscuelaApp.Models;
using EscuelaApp.Services;

namespace EscuelaApp.Controllers
{
    [Authorize]
    public class CalificacionesController : Controller
    {
        private readonly MongoService _mongo;

        public CalificacionesController(MongoService mongo) => _mongo = mongo;

        // GET /Calificaciones
        public async Task<IActionResult> Index()
        {
            var califs = await _mongo.ObtenerCalificacionesAsync();
            return View(califs);
        }

        // GET /Calificaciones/Crear?alumnoId=xxx
        public async Task<IActionResult> Crear(string? alumnoId)
        {
            var alumnos = await _mongo.ObtenerAlumnosAsync();
            ViewBag.Alumnos  = alumnos;
            ViewBag.AlumnoId = alumnoId; // pre-selecciona si viene desde detalle del alumno
            return View();
        }

        // POST /Calificaciones/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Calificacion calificacion)
        {
            // Quitar validación del campo de navegación
            ModelState.Remove("Alumno");

            if (!ModelState.IsValid)
            {
                ViewBag.Alumnos = await _mongo.ObtenerAlumnosAsync();
                return View(calificacion);
            }

            await _mongo.CrearCalificacionAsync(calificacion);
            TempData["Exito"] = "Calificación registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Calificaciones/Editar/id
        public async Task<IActionResult> Editar(string id)
        {
            var calif = await _mongo.ObtenerCalificacionPorIdAsync(id);
            if (calif == null) return NotFound();

            ViewBag.Alumnos = await _mongo.ObtenerAlumnosAsync();
            return View(calif);
        }

        // POST /Calificaciones/Editar/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string id, Calificacion calificacion)
        {
            ModelState.Remove("Alumno");

            if (!ModelState.IsValid)
            {
                ViewBag.Alumnos = await _mongo.ObtenerAlumnosAsync();
                return View(calificacion);
            }

            calificacion.Id = id;
            await _mongo.ActualizarCalificacionAsync(id, calificacion);
            TempData["Exito"] = "Calificación actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Calificaciones/Eliminar/id
        public async Task<IActionResult> Eliminar(string id)
        {
            var calif = await _mongo.ObtenerCalificacionPorIdAsync(id);
            if (calif == null) return NotFound();
            return View(calif);
        }

        // POST /Calificaciones/Eliminar/id
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(string id)
        {
            await _mongo.EliminarCalificacionAsync(id);
            TempData["Exito"] = "Calificación eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
