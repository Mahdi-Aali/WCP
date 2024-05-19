var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(cfg => {
    cfg.AddPolicy("default-cors", cp =>
    {
        string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!;

        cp
        .WithMethods(HttpMethods.Get)
        .WithOrigins(allowedOrigins)
        .AllowCredentials()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("default-cors");

app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Web configuration provider.");
});

app.MapGet("/{ProjectName}/{Version:int}/{FileName?}", async (HttpContext context, string ProjectName, int Version, string? FileName) =>
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Projects", ProjectName, $"V{Version}", FileName ?? "config.json");
    if (File.Exists(filePath))
    {
        string fileContent = await File.ReadAllTextAsync(filePath);
        await context.Response.WriteAsync(fileContent);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync("No config file with givin name and version found! - Error code: 404");
    }
});

app.MapGet("/Projects", async (HttpContext context) =>
{
    string projectsDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Projects");
    string[] projects = Directory.GetDirectories(projectsDirectoryPath).Select(s => Path.GetRelativePath(projectsDirectoryPath, s)).ToArray()!;
    if (projects is not null)
    {
        await context.Response.WriteAsJsonAsync(projects, projects.GetType());
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync("No projects found! Error code: 404");
    }
});


app.Run();