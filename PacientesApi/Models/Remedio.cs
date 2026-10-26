

using System.ComponentModel.DataAnnotations;

namespace PacientesApi.Models
{
    public class Remedio
    {
        public int RemedioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Dosagem { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FrequenciaUso { get; set; } = string.Empty;

        [Required]
        public int PacienteId { get; set; }

        public Paciente? Paciente { get; set; }
    }
}
