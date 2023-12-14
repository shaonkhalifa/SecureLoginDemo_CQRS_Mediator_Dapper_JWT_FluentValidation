
using FluentValidation;
using MadiatrProject.Command;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args);
Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;


builder.Services.AddDbContext<MDBContext>(opt=>opt.UseSqlServer(configuration.GetConnectionString("defaultconnections")));
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
builder.Services.AddSingleton<IValidator<Students>, StudentValidator>();


builder.Services.AddScoped<IDbConnection>(c =>
{
    var dbContext = c.GetRequiredService<MDBContext>();
    return dbContext.GetSqlConnection();
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
