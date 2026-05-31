using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication().AddCookie("ERPAdminCookies", options =>
{
    options.LoginPath = "/";
    options.AccessDeniedPath = "/dashboard";
    options.LogoutPath = "/logout";

    options.Cookie.Name = "ERPAdmin.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.MaxAge = TimeSpan.FromDays(90);
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    // Time settings
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = false;

    //options.Events = new CookieAuthenticationEvents
    //{
    //    OnValidatePrincipal = async context =>
    //    {
    //        var SessionService = context.HttpContext.RequestServices.GetRequiredService<MRSADMIN.Models.Session>();
    //        await SessionService.AdminCheckSession(context);
    //    }
    //};
});
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(50);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

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

app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
