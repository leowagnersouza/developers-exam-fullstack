using Infrastructure;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DEBUG: imprime a connection string lida pela aplicação (mascara a senha)
// Remova este bloco antes de comitar em produção.
try
{
    var cs = builder.Configuration.GetConnectionString("SQLConnection") ?? "(null)";
    var masked = Regex.Replace(cs, "(?i)(Password|Pwd)=([^^;]+)", "$1=****");
    Console.WriteLine($"[DEBUG] SQLConnection = {masked}");
}
catch (Exception ex)
{
    Console.WriteLine($"[DEBUG] Falha ao ler connection string: {ex.Message}");
}

// Configurações de serviços e dependências
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    // Permite chamadas de qualquer origem durante desenvolvimento / teste
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));

// Registra controllers MVC
builder.Services.AddControllers();

// Registra a camada de Infrastructure que configura o Entity Framework (SqlDbContext)
// e outros serviços relacionados (MediatR, repositórios, etc.).
builder.Services.AddInfrastructure(builder.Configuration);

// Configura o Swagger (OpenAPI) via Swashbuckle
// Comentários em XML são carregados para exibir documentação gerada a partir dos comentários do código.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (System.IO.File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
    catch
    {
        // Ignora falha ao carregar XML de comentários
    }
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Ativa Swagger UI apenas em ambiente de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // A UI ficará em /swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "INSPAND Exam API v1");
    });
}

// Endpoint de health-check simples
app.MapGet("/", () => Results.Ok("API is healthy!"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
