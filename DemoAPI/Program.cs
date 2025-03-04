using DemoAPI.BLL.Services;
using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.DAL.Database;
using DemoAPI.DAL.Repositories;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Middlewares;
using DemoAPI.Services;
using DemoAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<DemoDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailHelperService, MailHelperService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });

    // Permet d'ajouter le "cadenas" sur les routes
    // - Impl�mentation simple (Cadenas sur toutes les routes)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    // - Plus d'infos : 
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore?tab=readme-ov-file#add-security-definitions-and-requirements
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        //ValidateLifetime = true
    };
});

builder.Services.AddCors(options =>
    {
        // pas de s�curit�, tout le monde peut acc�der � l'API.
        // utile pour le d�veloppement mais � NE PAS UTILISER EN PRODUCTION
        options.AddPolicy("FFA", policy => {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });

        // Configuration de s�curit� pour le d�veloppement
        // uniquement les clients avec ces URLs sp�cifiques peuvent acc�der � l'API
        options.AddPolicy("Dev", policy =>
        {
            policy.WithOrigins("http://localhost:63342", "http://demo.be");
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Dev"); // on utilise la policy "Dev" d�finie plus haut

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication(); // check du token
app.UseAuthorization(); // v�rification des r�le ([Authorize])

app.MapControllers();

app.Run();
