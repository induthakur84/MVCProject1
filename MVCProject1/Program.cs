using Google.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MVCProject1.DataAccess.Data;
using MVCProject1.DataAccess.Repository.Repository;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Utility;
using NuGet.Protocol.Plugins;
using Stripe;
using System.Configuration;
using Twilio.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
/*
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders().
    AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();



/*builder.Services.AddScoped<StripeSetting>();*/
builder.Configuration.GetSection("Stripe");

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddHttpClient<ITwilioRestClient, TwilioClient>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "590848381653-dehcrk1h1p0eeig8mnc33vc1haaf3hik.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

// Facebook Login
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1312615256356262";
    options.AppSecret = "482f7573ab99f2dc0b43652bb5dcabfb";
});
builder.Services.Configure<EmailSetting>( builder.Configuration.GetSection("EmailSetting"));


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// in configure it is middleware in we use pipelining in which we use set wise set.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
/*StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe")["PublishableKey"];*/
StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe")["SecretKey"];

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

/// this is Action routing.
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
