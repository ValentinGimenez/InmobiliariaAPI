using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuarios/Logout";
		options.AccessDeniedPath = "/Home/Restringido"; 
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Admin")); 
});

builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
builder.Services.AddTransient<IRepositorioInquilino, RepositorioInquilino>();
builder.Services.AddTransient<IRepositorioContrato, RepositorioContrato>();
builder.Services.AddTransient<IRepositorioPago, RepositorioPago>();
builder.Services.AddTransient<IRepositorioTipoInmueble, RepositorioTipoInmueble>();
builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddTransient<IRepositorioAuditoria, RepositorioAuditoria>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();