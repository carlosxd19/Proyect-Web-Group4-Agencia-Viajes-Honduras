using AgenciaViajes.API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS (allow Angular dev server)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient for REST Countries
builder.Services.AddHttpClient("restcountries", c =>
{
    c.BaseAddress = new Uri("https://restcountries.com/");
});

// JWT Auth
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

// Firestore
builder.Services.AddSingleton(provider =>
{
    var fb = builder.Configuration.GetSection("Firebase");
    var credentialsPath = fb["CredentialsPath"];
    if (!string.IsNullOrWhiteSpace(credentialsPath))
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });
    }
    else
    {
        FirebaseApp.Create(); // uses GOOGLE_APPLICATION_CREDENTIALS env var
    }
    string projectId = fb["ProjectId"] ?? "";
    return new FirestoreDbBuilder { ProjectId = projectId }.Build();
});

// 🔹 Inicializar Firebase
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("config/interviajes-af2ed-firebase-adminsdk-fbsvc-4c0e09c3db.json")
    });
}

// 🔹 Config Firestore
builder.Services.AddSingleton<FirestoreDb>(sp =>
{
    return FirestoreDb.Create("interviajes-af2ed");
});

// App Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddSingleton<FirebaseUserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();