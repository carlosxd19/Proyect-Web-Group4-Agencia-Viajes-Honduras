using AgenciaViajes.API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CORS
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
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce tu token JWT con el prefijo Bearer",
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
// HttpClient
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
            FirebaseApp.Create();
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
builder.Services.AddScoped<FavoriteService>();

var app = builder.Build();

// =====================
// Archivos estáticos
// =====================
app.UseDefaultFiles(); // sirve index.html
app.UseStaticFiles();

// =====================
// Middleware
// =====================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// =====================
// Raíz opcional
// =====================
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();
