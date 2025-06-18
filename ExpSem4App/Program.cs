using ExpSem4App.Data;
using ExpSem4App.Repo;
using ExpSem4App.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC with views
builder.Services.AddControllersWithViews();

// --- Database Configuration ---
var connectionString = builder.Configuration.GetConnectionString("dfcs");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'dfcs' not found.");
}

// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection ---
// Repository Pattern
builder.Services.AddScoped<IExpenses, ExpRepo>();

// Services
builder.Services.AddScoped<DatabaseHelper>(_ => new DatabaseHelper(connectionString));
builder.Services.AddScoped<ExpensePredictionService>();

// --- Session Support (Optional) ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// --- Middleware Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Good for development
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Make sure session comes before authorization
app.UseAuthorization();

// Custom 404 error redirection (optional)
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/Home/NotFound";
        await next();
    }
});

// --- Default MVC Route ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Exp}/{action=Login}/{id?}");

// --- Auto-migrate on production ---
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
