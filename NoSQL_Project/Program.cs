// Author: Student1
using DotNetEnv;
using MongoDB.Driver;
using NoSQL_Project.Helpers;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;

// Load .env from solution root (traverse parent directories)
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// ── MongoDB ───────────────────────────────────────────────────────────────────
// IMongoClient is registered as SINGLETON because MongoClient is designed to be a
// long-lived, thread-safe connection pool. Creating one per request would spin up and
// tear down a full connection pool on every HTTP request — catastrophically wasteful.
// One instance per application lifetime is the correct pattern per MongoDB docs.
string connectionString = builder.Configuration["Mongo:ConnectionString"]
    ?? throw new InvalidOperationException(
        "MongoDB connection string is missing. " +
        "Add Mongo__ConnectionString=<your_atlas_uri> to the .env file at the solution root.");

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

// IMongoDatabase is registered as SCOPED because, while the MongoClient is long-lived,
// scoping the database handle to the HTTP request is safer for future multi-tenancy
// or middleware that might switch databases per request. Getting a database from a
// MongoClient is a lightweight O(1) operation (no network call), so the scoped
// lifetime adds no meaningful overhead.
string dbName = builder.Configuration["Mongo:Database"] ?? "NoDeskDB";
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));

// ── Repositories (Scoped) ─────────────────────────────────────────────────────
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ITicketRepository,   TicketRepository>();

// ── Services (Scoped) ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IEmployeeService,     EmployeeService>();
builder.Services.AddScoped<ITicketService,       TicketService>();
builder.Services.AddScoped<ITicketSearchService, TicketSearchService>(); // Individual feature

// ── Cookie authentication ─────────────────────────────────────────────────────
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath        = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan   = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Create MongoDB indexes once at startup (idempotent)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    await DatabaseInitializer.CreateIndexesAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// UseAuthentication MUST come before UseAuthorization so the auth cookie is decoded
// before the authorization middleware checks [Authorize] attributes.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
