using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PersonCrud.Api.Endpoints;
using PersonCrud.Api.Models;
using PersonCrud.Api.Validators;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// registering AppDbContext in the DI Container.
// It registered as Scoped lifetife.
string connectionString = "Data Source=Person.db";
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

var app = builder.Build();

// Aplicar migraciones y crear Person.db automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors(MyAllowSpecificOrigins);

app.MapPersonEndpoints();

app.Run();