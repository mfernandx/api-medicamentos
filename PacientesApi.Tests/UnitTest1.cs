
using System.ComponentModel.DataAnnotations;
using PacientesApi.Models;
using Xunit;

namespace PacientesApi.Tests
{
    public class ModelosTests
    {
        [Fact]
        public void Paciente_ComDadosValidos_DevePassarNaValidacao()
        {
            var paciente = new Paciente
            {
                Nome = "Maria Silva",
                DataNascimento = new DateTime(2000, 5, 15),
                CPF = "12345678901"
            };

            var contexto = new ValidationContext(paciente);
            var resultados = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(
                paciente,
                contexto,
                resultados,
                true
            );

            Assert.True(valido);
        }

        [Fact]
        public void Paciente_ComCPFInvalido_DeveFalharNaValidacao()
        {
            var paciente = new Paciente
            {
                Nome = "Maria Silva",
                DataNascimento = new DateTime(2000, 5, 15),
                CPF = "123"
            };

            var contexto = new ValidationContext(paciente);
            var resultados = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(
                paciente,
                contexto,
                resultados,
                true
            );

            Assert.False(valido);
        }

        [Fact]
        public void Remedio_SemNome_DeveFalharNaValidacao()
        {
            var remedio = new Remedio
            {
                Nome = "",
                Dosagem = "500 mg",
                FrequenciaUso = "A cada 8 horas",
                PacienteId = 1
            };

            var contexto = new ValidationContext(remedio);
            var resultados = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(
                remedio,
                contexto,
                resultados,
                true
            );

            Assert.False(valido);
        }
    }
}

