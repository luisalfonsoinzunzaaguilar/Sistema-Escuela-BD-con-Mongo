using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EscuelaApp.Models
{
    public class Alumno
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La matrícula es obligatoria")]
        public string Matricula { get; set; } = "";

        [Required(ErrorMessage = "La carrera es obligatoria")]
        public string Carrera { get; set; } = "";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
