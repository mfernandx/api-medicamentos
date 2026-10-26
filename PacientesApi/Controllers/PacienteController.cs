
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using PacientesApi.Data;
using PacientesApi.Models;

namespace PacientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly OracleConnectionFactory _factory;

        public PacienteController(OracleConnectionFactory factory)
        {
            _factory = factory;
        }

        // GET: api/Paciente
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var pacientes = new List<Paciente>();

            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"SELECT PACIENTE_ID, NOME, DATA_NASCIMENTO, CPF
                  FROM PACIENTES
                  ORDER BY PACIENTE_ID",
                conexao);

            using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
            {
                pacientes.Add(new Paciente
                {
                    PacienteId = Convert.ToInt32(leitor["PACIENTE_ID"]),
                    Nome = leitor["NOME"].ToString()!,
                    DataNascimento = Convert.ToDateTime(
                        leitor["DATA_NASCIMENTO"]),
                    CPF = leitor["CPF"].ToString()!
                });
            }

            return Ok(pacientes);
        }

        // GET: api/Paciente/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"SELECT PACIENTE_ID, NOME, DATA_NASCIMENTO, CPF
                  FROM PACIENTES
                  WHERE PACIENTE_ID = :id",
                conexao);

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            using var leitor = await comando.ExecuteReaderAsync();

            if (!await leitor.ReadAsync())
                return NotFound(new { mensagem = "Paciente não encontrado." });

            var paciente = new Paciente
            {
                PacienteId = Convert.ToInt32(leitor["PACIENTE_ID"]),
                Nome = leitor["NOME"].ToString()!,
                DataNascimento = Convert.ToDateTime(
                    leitor["DATA_NASCIMENTO"]),
                CPF = leitor["CPF"].ToString()!
            };

            return Ok(paciente);
        }

        // POST: api/Paciente
        [HttpPost]
        public async Task<IActionResult> Criar(Paciente paciente)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"INSERT INTO PACIENTES
                    (NOME, DATA_NASCIMENTO, CPF)
                  VALUES
                    (:nome, :dataNascimento, :cpf)
                  RETURNING PACIENTE_ID INTO :id",
                conexao);

            comando.Parameters.Add(
                "nome", OracleDbType.Varchar2, 100).Value = paciente.Nome;

            comando.Parameters.Add(
                "dataNascimento", OracleDbType.Date).Value =
                paciente.DataNascimento.Date;

            comando.Parameters.Add(
                "cpf", OracleDbType.Varchar2, 11).Value = paciente.CPF;

            var parametroId = comando.Parameters.Add(
                "id", OracleDbType.Decimal);

            parametroId.Direction = System.Data.ParameterDirection.Output;

            await comando.ExecuteNonQueryAsync();

            paciente.PacienteId = Convert.ToInt32(
                parametroId.Value.ToString());

            return CreatedAtAction(
                nameof(GetPorId),
                new { id = paciente.PacienteId },
                paciente);
        }

        // PUT: api/Paciente/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(
            int id, Paciente paciente)
        {
            if (id != paciente.PacienteId)
            {
                return BadRequest(new
                {
                    mensagem = "O ID da URL deve ser igual ao ID do paciente."
                });
            }

            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                @"UPDATE PACIENTES
                  SET NOME = :nome,
                      DATA_NASCIMENTO = :dataNascimento,
                      CPF = :cpf
                  WHERE PACIENTE_ID = :id",
                conexao);

            comando.Parameters.Add(
                "nome", OracleDbType.Varchar2, 100).Value = paciente.Nome;

            comando.Parameters.Add(
                "dataNascimento", OracleDbType.Date).Value =
                paciente.DataNascimento.Date;

            comando.Parameters.Add(
                "cpf", OracleDbType.Varchar2, 11).Value = paciente.CPF;

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            int linhas = await comando.ExecuteNonQueryAsync();

            if (linhas == 0)
                return NotFound(new { mensagem = "Paciente não encontrado." });

            return NoContent();
        }

        // DELETE: api/Paciente/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            using var conexao = _factory.CreateConnection();
            await conexao.OpenAsync();

            using var comando = new OracleCommand(
                "DELETE FROM PACIENTES WHERE PACIENTE_ID = :id",
                conexao);

            comando.Parameters.Add("id", OracleDbType.Int32).Value = id;

            int linhas = await comando.ExecuteNonQueryAsync();

            if (linhas == 0)
                return NotFound(new { mensagem = "Paciente não encontrado." });

            return NoContent();
        }
    }
}

