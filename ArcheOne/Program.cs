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
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);//You can set Time   

});
//builder.Services.AddIdentityCore<IdentityUser>()
//       .AddDefaultTokenProviders();

//builder.Services.AddAuthentication();

Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "MyCookieDemo";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Set the desired expiration time
    options.SlidingExpiration = true;
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
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=LogIn}/{id?}");

app.Run();
