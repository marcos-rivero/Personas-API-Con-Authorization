using System.ComponentModel.DataAnnotations;

namespace PersonasAPI.Modelo
{
    public class Personas
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [MaxLength(128)]
        public string Direccion { get; set; }

    }
}
