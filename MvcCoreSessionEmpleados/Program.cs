using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
//añadimos el memory cache para el cache personalizado
builder.Services.AddMemoryCache();

string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
string connectionStringAzure = builder.Configuration.GetConnectionString("SqlAzure");

builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionStringAzure));

builder.Services.AddTransient<RepositoryEmpleados>();

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
app.UseRouting();

app.UseAuthorization();
app.MapStaticAssets();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
