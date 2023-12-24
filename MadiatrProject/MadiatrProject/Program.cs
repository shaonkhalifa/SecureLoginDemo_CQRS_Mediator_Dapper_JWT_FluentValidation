
using FluentValidation;
using FluentValidation.AspNetCore;
using MadiatrProject.Attributes;
using MadiatrProject.Behaviour;
using MadiatrProject.Cache;
using MadiatrProject.Command;
using MadiatrProject.DbContexts;
using MadiatrProject.Fillter;
using MadiatrProject.Middlewares;
using MadiatrProject.Model;
using MadiatrProject.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;


builder.Services.AddDbContext<MDBContext>(opt=>opt.UseSqlServer(configuration.GetConnectionString("defaultconnections")));
builder.Services.AddDbContext<SDBContext>(opt => opt.UseSqlite(configuration.GetConnectionString("sqliteDbConnections")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    string? connection = builder.Configuration.GetConnectionString("Radis");
    options.Configuration = connection;//"localhost:6379";
});
//builder.Services.AddSingleton<IConfigureOptions<DbContextOptions<MDBContext>>, DbInitializer>();
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<AppSettings>();
builder.Services.AddScoped<AuthorizeAttribute>();
builder.Services.AddTransient<UserAuthenticationService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior <,>), typeof(ErrorCodeMappingExceptionFilter<,>));


//builder.Services.AddScoped<IDbConnection>(c =>
//{
//    var dbContext = c.GetRequiredService<MDBContext>();
//    return dbContext.GetSqlConnection();
//});
//builder.Services.AddControllers()
//       .AddJsonOptions(option =>
//       {
//       option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
//       option.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
//       //       });


       var appSettingsSection = configuration.GetSection("AppSettings");

//jwt
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.key);
builder.Services.AddAuthentication(au =>
{
    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.RequireHttpsMetadata = false;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,

    };
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
   
});



var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    services.GetRequiredService<MDBContext>();
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<ExecptionHandlingMiddleware>();

app.UseCors(x =>
{
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
});

app.MapControllers();

app.Run();
