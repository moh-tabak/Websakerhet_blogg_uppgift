using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Uppgift2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<DbConfig>(builder.Configuration.GetSection("DbConfig"));
builder.Services.AddSingleton<IDbService, DbService>();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(ops =>
//{
//    ops.Cookie.Name = "UserAuthCookie";
//    ops.Cookie.IsEssential = true;
//    ops.ClaimsIssuer = "webbsakerhet_uppgift2";
//    ops.SlidingExpiration = true;
//    ops.ExpireTimeSpan = TimeSpan.FromHours(1);
//});
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();