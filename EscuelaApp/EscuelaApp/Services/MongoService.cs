using MongoDB.Driver;
using MongoDB.Bson;
using EscuelaApp.Models;

namespace EscuelaApp.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _db;

        public MongoService(IConfiguration config)
        {
            var uri = config["MongoDB:URI"];
            var client = new MongoClient(uri);
            _db = client.GetDatabase(config["MongoDB:Database"]);

            CrearIndices();
        }

        // ── Colecciones ──────────────────────────────────────────
        private IMongoCollection<Alumno> Alumnos =>
            _db.GetCollection<Alumno>("alumnos");

        private IMongoCollection<Calificacion> Calificaciones =>
            _db.GetCollection<Calificacion>("calificaciones");

        private IMongoCollection<Usuario> Usuarios =>
            _db.GetCollection<Usuario>("usuarios");

        // ── Índices (se crean una vez al arrancar) ───────────────
        private void CrearIndices()
        {
            // Email único en alumnos
            var idxAlumno = Builders<Alumno>.IndexKeys.Ascending(a => a.Email);
            Alumnos.Indexes.CreateOne(
                new CreateIndexModel<Alumno>(idxAlumno,
                    new CreateIndexOptions { Unique = true }));

            // Email único en usuarios
            var idxUsuario = Builders<Usuario>.IndexKeys.Ascending(u => u.Email);
            Usuarios.Indexes.CreateOne(
                new CreateIndexModel<Usuario>(idxUsuario,
                    new CreateIndexOptions { Unique = true }));

            // Matrícula única en alumnos
            var idxMatricula = Builders<Alumno>.IndexKeys.Ascending(a => a.Matricula);
            Alumnos.Indexes.CreateOne(
                new CreateIndexModel<Alumno>(idxMatricula,
                    new CreateIndexOptions { Unique = true }));
        }

        // ════════════════════════════════════════════════════════
        // ALUMNOS — CRUD
        // ════════════════════════════════════════════════════════

        public async Task<List<Alumno>> ObtenerAlumnosAsync() =>
            await Alumnos.Find(_ => true).SortBy(a => a.Nombre).ToListAsync();

        public async Task<Alumno?> ObtenerAlumnoPorIdAsync(string id) =>
            await Alumnos.Find(a => a.Id == id).FirstOrDefaultAsync();

        public async Task CrearAlumnoAsync(Alumno alumno) =>
            await Alumnos.InsertOneAsync(alumno);

        public async Task ActualizarAlumnoAsync(string id, Alumno alumno) =>
            await Alumnos.ReplaceOneAsync(a => a.Id == id, alumno);

        public async Task EliminarAlumnoAsync(string id)
        {
            // Al eliminar alumno, se eliminan también sus calificaciones
            await Calificaciones.DeleteManyAsync(c => c.AlumnoId == id);
            await Alumnos.DeleteOneAsync(a => a.Id == id);
        }

        // ════════════════════════════════════════════════════════
        // CALIFICACIONES — CRUD
        // ════════════════════════════════════════════════════════

        public async Task<List<Calificacion>> ObtenerCalificacionesAsync()
        {
            var califs = await Calificaciones.Find(_ => true)
                .SortByDescending(c => c.Fecha).ToListAsync();

            // Unir con datos del alumno (join manual)
            foreach (var c in califs)
                c.Alumno = await ObtenerAlumnoPorIdAsync(c.AlumnoId);

            return califs;
        }

        public async Task<List<Calificacion>> ObtenerCalificacionesPorAlumnoAsync(string alumnoId)
        {
            var califs = await Calificaciones
                .Find(c => c.AlumnoId == alumnoId)
                .SortByDescending(c => c.Fecha).ToListAsync();

            var alumno = await ObtenerAlumnoPorIdAsync(alumnoId);
            foreach (var c in califs) c.Alumno = alumno;

            return califs;
        }

        public async Task<Calificacion?> ObtenerCalificacionPorIdAsync(string id)
        {
            var c = await Calificaciones.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (c != null) c.Alumno = await ObtenerAlumnoPorIdAsync(c.AlumnoId);
            return c;
        }

        public async Task CrearCalificacionAsync(Calificacion calificacion) =>
            await Calificaciones.InsertOneAsync(calificacion);

        public async Task ActualizarCalificacionAsync(string id, Calificacion calificacion) =>
            await Calificaciones.ReplaceOneAsync(c => c.Id == id, calificacion);

        public async Task EliminarCalificacionAsync(string id) =>
            await Calificaciones.DeleteOneAsync(c => c.Id == id);

        // ════════════════════════════════════════════════════════
        // USUARIOS — Autenticación
        // ════════════════════════════════════════════════════════

        public async Task<Usuario?> ObtenerUsuarioPorEmailAsync(string email) =>
            await Usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task CrearUsuarioAsync(Usuario usuario) =>
            await Usuarios.InsertOneAsync(usuario);

        public async Task<bool> ExisteUsuarioAsync(string email) =>
            await Usuarios.Find(u => u.Email == email).AnyAsync();

        // ════════════════════════════════════════════════════════
        // SEED — Datos de ejemplo
        // ════════════════════════════════════════════════════════

        public async Task SeedAsync()
        {
            // Usuario demo
            if (!await ExisteUsuarioAsync("demo@demo.com"))
            {
                await CrearUsuarioAsync(new Usuario
                {
                    Email = "demo@demo.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234")
                });
            }

            // Alumnos de ejemplo
            var totalAlumnos = await Alumnos.CountDocumentsAsync(_ => true);
            if (totalAlumnos == 0)
            {
                var alumnos = new List<Alumno>
                {
                    new() { Nombre = "Ana García López",     Email = "ana@escuela.com",    Matricula = "2024001", Carrera = "Ingeniería en Sistemas" },
                    new() { Nombre = "Carlos Méndez Ruiz",   Email = "carlos@escuela.com", Matricula = "2024002", Carrera = "Administración" },
                    new() { Nombre = "Sofía Torres Vega",    Email = "sofia@escuela.com",  Matricula = "2024003", Carrera = "Contabilidad" },
                    new() { Nombre = "Luis Ramírez Castro",  Email = "luis@escuela.com",   Matricula = "2024004", Carrera = "Ingeniería en Sistemas" },
                };
                await Alumnos.InsertManyAsync(alumnos);

                // Calificaciones de ejemplo
                var anaId    = alumnos[0].Id!;
                var carlosId = alumnos[1].Id!;

                var califs = new List<Calificacion>
                {
                    new() { AlumnoId = anaId,    Materia = "Matemáticas",      Calific = 9.5, Periodo = "2026-1" },
                    new() { AlumnoId = anaId,    Materia = "Programación",      Calific = 10,  Periodo = "2026-1" },
                    new() { AlumnoId = carlosId, Materia = "Administración I",  Calific = 8.0, Periodo = "2026-1" },
                    new() { AlumnoId = carlosId, Materia = "Contabilidad",      Calific = 7.5, Periodo = "2026-1" },
                };
                await Calificaciones.InsertManyAsync(califs);
            }
        }
    }
}
