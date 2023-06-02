using ArcheOne;
using ArcheOne.Database.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
string connString = builder.Configuration["ConnectionStrings:EntitiesConnection"];
builder.Services.AddDbContext<ArcheOneDbContext>(options =>
{
    options.UseSqlServer(connString);
});

builder.Services.DIScopes();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(options =>
 {
     options.Cookie.Name = "RememberMecookie1"; // cookie name
     options.Cookie.SameSite = SameSiteMode.None; // view where the cookie will be issued for the first time
     options.ExpireTimeSpan = TimeSpan.FromDays(30); // time for the cookei to last in the browser
     options.SlidingExpiration = true; // the cookie would be re-issued on any request half way through the ExpireTimeSpan
     options.EventsType = typeof(CookieAuthEvent);
 });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);//You can set Time   

});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseCookiePolicy();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=LogIn}/{id?}");

app.Run();
