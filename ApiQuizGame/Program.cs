using ApiQuizGame.Data;
using ApiQuizGame.Data.Seed;
using ApiQuizGame.Middlewares;
using ApiQuizGame.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Quiz Game API",
        Version = "v1",
        Description = "REST API for quiz game management",
        Contact = new() { Name = "Jairo", Email = "jairo@test.com" }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<QuizGameDbContext>(options =>

options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null)));

// HttpClient para Gemini AI
builder.Services.AddHttpClient<AiQuestionService>();

builder.Services.AddScoped<AiQuestionService>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<QuizGameDbContext>();

    await context.Database.MigrateAsync();

    await QuizGameSeeder.SeedAsync(context);

    // Si algún nivel no tiene preguntas, generarlas con IA
    var aiService = scope.ServiceProvider.GetRequiredService<AiQuestionService>();
    var categories = await context.Categories.ToListAsync();

    foreach (var category in categories.OrderBy(c => c.Level))
    {
        var hasQuestions = await context.Questions
            .AnyAsync(q => q.CategoryId == category.Id);

        if (!hasQuestions)
        {
            Console.WriteLine($"Generando pregunta con IA para nivel {category.Level}...");
            await aiService.GenerateAndSaveQuestionAsync(category.Id, category.Level);
            await Task.Delay(5000); // Pausa entre niveles para evitar rate limit
        }
    }

}
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
