using AgenciaViajes.API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CORS
// =====================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .WithOrigins(
            "http://localhost:4200",
            "http://localhost:5173",
            "http://localhost:3000",
            "https://localhost:4200")
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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Agencia de Viajes — API (Firebase/Firestore)",
        Version = "v1",
        Description = "Clientes, países, viajes y favoritos sobre Firestore"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer {token}",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" }}, Array.Empty<string>() }
    });
});

// =====================
// HttpClient (restcountries)
// =====================
builder.Services.AddHttpClient("restcountries", c =>
{
    c.BaseAddress = new Uri("https://restcountries.com/");
});

// =====================
// (Opcional) JWT propio
// =====================
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
if (!string.IsNullOrWhiteSpace(jwtKey))
{
    var key = Encoding.UTF8.GetBytes(jwtKey);
    builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
}
// =====================
// Firebase Admin + Firestore  (⚠️ fix + validaciones de credenciales)
// =====================
builder.Services.AddSingleton(provider =>
{
    var fb = builder.Configuration.GetSection("Firebase");
    var credentialsPath = fb["CredentialsPath"]
                          ?? "config/interviajes-af2ed-firebase-adminsdk-fbsvc-b5dc9c7731.json";
    var projectId = fb["ProjectId"] ?? "interviajes-af2ed";

    // 0) Validaciones rápidas de archivo
    if (!File.Exists(credentialsPath))
        throw new FileNotFoundException($"No se encontró el archivo de credenciales en: {credentialsPath}");

    // 0.1) Comprobar que realmente es una service account
    //     (evita que, por error, se use un firebase-config.json del front)
    var json = File.ReadAllText(credentialsPath);
    if (!json.Contains("\"type\"") || !json.Contains("\"service_account\"") || !json.Contains("\"private_key\""))
        throw new InvalidOperationException(
            $"El archivo de credenciales no es de tipo 'service_account' o está incompleto: {credentialsPath}");

    // Limpia posibles variables del emulador
    Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", null);

    // 1) Carga del JSON y FORZAR SCOPES de Firestore
    var baseCred = GoogleCredential.FromFile(credentialsPath);
    var scopedCred = baseCred.IsCreateScopedRequired
        ? baseCred.CreateScoped(FirestoreClient.DefaultScopes) // << scopes correctos (datastore, cloud-platform)
        : baseCred;

    // 2) Inicializa FirebaseApp (si vas a usar FirebaseAdmin.Auth, etc.)
    if (FirebaseApp.DefaultInstance == null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = scopedCred,
            ProjectId = projectId
        });
    }

    // 3) Construye FirestoreClient con esa credencial (gRPC con token OAuth2)
    var client = new FirestoreClientBuilder
    {
        Credential = scopedCred
    }.Build();

    // 4) Crea FirestoreDb con el cliente ya autenticado
    return FirestoreDb.Create(projectId, client);
});

// =====================
// Services (DI)
// =====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<FirebaseService>();

var app = builder.Build();

// =====================
// Archivos estáticos (front)
// =====================
app.UseDefaultFiles();
app.UseStaticFiles();

// =====================
// Swagger
// =====================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agencia de Viajes API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Swagger — Agencia de Viajes";
});

app.UseCors();
app.UseHttpsRedirection();

if (!string.IsNullOrWhiteSpace(jwtKey))
    app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

// ============= DEBUG de conexión =============
// Escribe un doc de prueba en /pruebas para verificar credenciales
app.MapGet("/debug/firestore", async (FirestoreDb db) =>
{
    var id = Guid.NewGuid().ToString("N");
    await db.Collection("pruebas").Document(id).SetAsync(new
    {
        ok = true,
        when = DateTime.UtcNow
    });
    return Results.Ok(new { id });
});

app.MapGet("/", () => Results.Redirect("/index.html"));
app.Run();
