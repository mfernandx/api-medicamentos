
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using PacientesApi.Data;
using PacientesApi.Models;

namespace PacientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemedioController : ControllerBase
    {
        private readonly OracleConnectionFactory _factory;

        public RemedioController(OracleConnectionFactory factory)
        {
            _factory = factory;
        }

        // GET: api/Remedio
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var remedios = new List<Remedio>();

            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"SELECT REMEDIO_ID, NOME, DOSAGEM,
                         FREQUENCIA_USO, PACIENTE_ID
                  FROM REMEDIOS
                  ORDER BY REMEDIO_ID",
                conexao);

            using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
            {
                remedios.Add(new Remedio
                {
                    RemedioId = Convert.ToInt32(leitor["REMEDIO_ID"]),
                    Nome = leitor["NOME"].ToString()!,
                    Dosagem = leitor["DOSAGEM"].ToString()!,
                    FrequenciaUso = leitor["FREQUENCIA_USO"].ToString()!,
                    PacienteId = Convert.ToInt32(leitor["PACIENTE_ID"])
                });
            }

            return Ok(remedios);
        }

        // GET: api/Remedio/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"SELECT REMEDIO_ID, NOME, DOSAGEM,
                         FREQUENCIA_USO, PACIENTE_ID
                  FROM REMEDIOS
                  WHERE REMEDIO_ID = :id",
                conexao);

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            using var leitor = await comando.ExecuteReaderAsync();

            if (!await leitor.ReadAsync())
                return NotFound(new { mensagem = "Remédio não encontrado." });

            var remedio = new Remedio
            {
                RemedioId = Convert.ToInt32(leitor["REMEDIO_ID"]),
                Nome = leitor["NOME"].ToString()!,
                Dosagem = leitor["DOSAGEM"].ToString()!,
                FrequenciaUso = leitor["FREQUENCIA_USO"].ToString()!,
                PacienteId = Convert.ToInt32(leitor["PACIENTE_ID"])
            };

            return Ok(remedio);
        }

        // POST: api/Remedio
        [HttpPost]
        public async Task<IActionResult> Criar(Remedio remedio)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            if (!await PacienteExiste(conexao, remedio.PacienteId))
            {
                return BadRequest(new
                {
                    mensagem = "O paciente informado não existe."
                });
            }

            using var comando = new OracleCommand(
                @"INSERT INTO REMEDIOS
                    (NOME, DOSAGEM, FREQUENCIA_USO, PACIENTE_ID)
                  VALUES
                    (:nome, :dosagem, :frequencia, :pacienteId)
                  RETURNING REMEDIO_ID INTO :id",
                conexao);

            comando.Parameters.Add(
                "nome", OracleDbType.Varchar2, 100).Value = remedio.Nome;

            comando.Parameters.Add(
                "dosagem", OracleDbType.Varchar2, 50).Value = remedio.Dosagem;

            comando.Parameters.Add(
                "frequencia", OracleDbType.Varchar2, 100).Value =
                remedio.FrequenciaUso;

            comando.Parameters.Add(
                "pacienteId", OracleDbType.Int32).Value = remedio.PacienteId;

            var parametroId = comando.Parameters.Add(
                "id", OracleDbType.Decimal);

            parametroId.Direction = System.Data.ParameterDirection.Output;

            await comando.ExecuteNonQueryAsync();

            remedio.RemedioId = Convert.ToInt32(
                parametroId.Value.ToString());

            return CreatedAtAction(
                nameof(GetPorId),
                new { id = remedio.RemedioId },
                remedio);
        }

        // PUT: api/Remedio/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, Remedio remedio)
        {
            if (id != remedio.RemedioId)
            {
                return BadRequest(new
                {
                    mensagem = "O ID da URL deve ser igual ao ID do remédio."
                });
            }

            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            if (!await PacienteExiste(conexao, remedio.PacienteId))
            {
                return BadRequest(new
                {
                    mensagem = "O paciente informado não existe."
                });
            }

            using var comando = new OracleCommand(
                @"UPDATE REMEDIOS
                  SET NOME = :nome,
                      DOSAGEM = :dosagem,
                      FREQUENCIA_USO = :frequencia,
                      PACIENTE_ID = :pacienteId
                  WHERE REMEDIO_ID = :id",
                conexao);

            comando.Parameters.Add(
                "nome", OracleDbType.Varchar2, 100).Value = remedio.Nome;

            comando.Parameters.Add(
                "dosagem", OracleDbType.Varchar2, 50).Value = remedio.Dosagem;

            comando.Parameters.Add(
                "frequencia", OracleDbType.Varchar2, 100).Value =
                remedio.FrequenciaUso;

            comando.Parameters.Add(
                "pacienteId", OracleDbType.Int32).Value = remedio.PacienteId;

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            int linhas = await comando.ExecuteNonQueryAsync();

            if (linhas == 0)
                return NotFound(new { mensagem = "Remédio não encontrado." });

            return NoContent();
        }

        // DELETE: api/Remedio/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                "DELETE FROM REMEDIOS WHERE REMEDIO_ID = :id",
                conexao);

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            int linhas = await comando.ExecuteNonQueryAsync();

            if (linhas == 0)
                return NotFound(new { mensagem = "Remédio não encontrado." });

            return NoContent();
        }

        private static async Task<bool> PacienteExiste(
            OracleConnection conexao, int pacienteId)
        {
            using var comando = new OracleCommand(
                @"SELECT COUNT(*)
                  FROM PACIENTES
                  WHERE PACIENTE_ID = :id",
                conexao);

            comando.Parameters.Add("id", OracleDbType.Int32).Value = pacienteId;

            var resultado = await comando.ExecuteScalarAsync();

            return Convert.ToInt32(resultado) > 0;
        }
    }
}

