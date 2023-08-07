using ArcheOne;
using ArcheOne.Database.Entities;
using ArcheOne.Filters;
using ArcheOne.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(ActionFilters));
});

builder.Services.AddDbContext<ArcheOneDbContext>(x => x.UseSqlServer(builder.Configuration["ConnectionStrings:EntitiesConnection"]));

builder.Services.AddSession(x => x.IdleTimeout = TimeSpan.FromMinutes(30));//You can set Time   


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(options =>
 {
     options.Cookie.Name = "RememberMeCookie"; // cookie name
     //options.Cookie.SameSite = SameSiteMode.None; // view where the cookie will be issued for the first time
     //options.ExpireTimeSpan = TimeSpan.FromDays(30); // time for the cookei to last in the browser
     options.SlidingExpiration = true; // the cookie would be re-issued on any request half way through the ExpireTimeSpan
 });

builder.Services.AddDistributedMemoryCache();

builder.Services.DIScopes();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/PageNotFound");
}
app.UseCookiePolicy();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=LogIn}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
