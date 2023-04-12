using ASP_202.Data;
using ASP_202.Services;
using ASP_202.Services.Hash;
using ASP_202.Services.Kdf;
using ASP_202.Services.Random;
using Microsoft.EntityFrameworkCore;
// using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TimeService>();
builder.Services.AddTransient<DateService>();
builder.Services.AddScoped<DtService>();

builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IRandomService, RandomServiceV1>();
builder.Services.AddSingleton<IKdfService, HashKdfService>();


String? connectionString = builder.Configuration.GetConnectionString("MainDb");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString)
);

// MySQL
/*
connectionString = builder.Configuration.GetConnectionString("PlanetDb");
MySqlConnection connection = new MySqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));
        // new MySqlServerVersion(new Version(8, 0, 23))));
*/

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
