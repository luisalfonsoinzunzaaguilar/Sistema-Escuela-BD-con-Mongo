using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EscuelaApp.Models;
using EscuelaApp.Services;

namespace EscuelaApp.Controllers
{
    [Authorize]
    public class AlumnosController : Controller
    {
        private readonly MongoService _mongo;

        public AlumnosController(MongoService mongo) => _mongo = mongo;

        // GET /Alumnos
        public async Task<IActionResult> Index()
        {
            var alumnos = await _mongo.ObtenerAlumnosAsync();
            return View(alumnos);
        }

        // GET /Alumnos/Detalle/id
        public async Task<IActionResult> Detalle(string id)
        {
            var alumno = await _mongo.ObtenerAlumnoPorIdAsync(id);
            if (alumno == null) return NotFound();

            var calificaciones = await _mongo.ObtenerCalificacionesPorAlumnoAsync(id);
            ViewBag.Calificaciones = calificaciones;
            return View(alumno);
        }

        // GET /Alumnos/Crear
        public IActionResult Crear() => View();

        // POST /Alumnos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Alumno alumno)
        {
            if (!ModelState.IsValid) return View(alumno);

            try
            {
                await _mongo.CrearAlumnoAsync(alumno);
                TempData["Exito"] = "Alumno creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "El email o matrícula ya existe.");
                return View(alumno);
            }
        }

        // GET /Alumnos/Editar/id
        public async Task<IActionResult> Editar(string id)
        {
            var alumno = await _mongo.ObtenerAlumnoPorIdAsync(id);
            if (alumno == null) return NotFound();
            return View(alumno);
        }

        // POST /Alumnos/Editar/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string id, Alumno alumno)
        {
            if (!ModelState.IsValid) return View(alumno);

            alumno.Id = id;
            await _mongo.ActualizarAlumnoAsync(id, alumno);
            TempData["Exito"] = "Alumno actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Alumnos/Eliminar/id
        public async Task<IActionResult> Eliminar(string id)
        {
            var alumno = await _mongo.ObtenerAlumnoPorIdAsync(id);
            if (alumno == null) return NotFound();
            return View(alumno);
        }

        // POST /Alumnos/Eliminar/id
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(string id)
        {
            await _mongo.EliminarAlumnoAsync(id);
            TempData["Exito"] = "Alumno eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
