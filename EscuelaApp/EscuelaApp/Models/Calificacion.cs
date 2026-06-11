using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EscuelaApp.Models
{
    public class Calificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Referencia al Alumno (ObjectId como string)
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "El alumno es obligatorio")]
        public string AlumnoId { get; set; } = "";

        [Required(ErrorMessage = "La materia es obligatoria")]
        public string Materia { get; set; } = "";

        [Required(ErrorMessage = "La calificación es obligatoria")]
        [Range(0, 10, ErrorMessage = "La calificación debe estar entre 0 y 10")]
        public double Calific { get; set; }

        [Required(ErrorMessage = "El periodo es obligatorio")]
        public string Periodo { get; set; } = "";

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Campo de navegación (no se guarda en Mongo, solo para mostrar en vistas)
        [BsonIgnore]
        public Alumno? Alumno { get; set; }
    }
}
