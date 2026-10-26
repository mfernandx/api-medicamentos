
using System.ComponentModel.DataAnnotations;

namespace PacientesApi.Models
{
    public class Paciente
    {
        public int PacienteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string CPF { get; set; } = string.Empty;

        public List<Remedio> Remedios { get; set; } = new List<Remedio>();
    }
}

