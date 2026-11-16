using DataAccessLevel.Context;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLevel.Servicios;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ControlAsistenciaContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionCTP")));

// Add services to the container.
builder.Services.AddScoped<UsuarioServicio>();
builder.Services.AddScoped<ProfesorServicio>();
builder.Services.AddScoped<EspecialidadServicio>();
builder.Services.AddScoped<AulaServicio>();
builder.Services.AddScoped<UbicacionServico>();
builder.Services.AddScoped<AsistenciaServicio>();
builder.Services.AddScoped<AusenciaService>();
builder.Services.AddScoped<ReporteServicio>();
builder.Services.AddScoped<PdfReporteService>();
builder.Services.AddScoped<HistorialCodigoServicio>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LogoutPath = "/Auth/Logout";
    });

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
