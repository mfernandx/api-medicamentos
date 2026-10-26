
using PacientesApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Serviços da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Fábrica de conexões Oracle
builder.Services.AddSingleton<OracleConnectionFactory>();

var app = builder.Build();

// Documentação da API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
