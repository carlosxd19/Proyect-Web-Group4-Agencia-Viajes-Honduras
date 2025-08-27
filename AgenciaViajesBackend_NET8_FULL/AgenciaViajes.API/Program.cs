using AgenciaViajes.API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CORS (para Angular dev server)
// =====================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// =====================
// Controllers + Swagger
// =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con soporte de JWT
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // ✅ permite usar [SwaggerOperation] en controladores

    // 🔐 Configuración de seguridad JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce tu token JWT con el prefijo Bearer (ej: Bearer eyJhbGciOiJI...)",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// =====================
// HttpClient para RestCountries
// =====================
builder.Services.AddHttpClient("restcountries", c =>
{
    c.BaseAddress = new Uri("https://restcountries.com/");
});

// =====================
// JWT Auth
// =====================
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// =====================
// Firestore
// =====================
builder.Services.AddSingleton(provider =>
{
    var fb = builder.Configuration.GetSection("Firebase");
    var credentialsPath = fb["CredentialsPath"];

    // ✅ Evitar inicializar FirebaseApp más de una vez
    if (FirebaseApp.DefaultInstance == null)
    {
        if (!string.IsNullOrWhiteSpace(credentialsPath))
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialsPath)
            });
        }
        else
        {
            FirebaseApp.Create(); // usa GOOGLE_APPLICATION_CREDENTIALS env var
        }
    }

    string projectId = fb["ProjectId"] ?? "";
    return new FirestoreDbBuilder { ProjectId = projectId }.Build();
});

// =====================
// App Services
// =====================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<CountryService>();

// =====================
// App
// =====================
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// 🚀 Redirige la raíz directamente a Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
