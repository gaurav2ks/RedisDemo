using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedisDemo.Data;
using StackExchange.Redis;
using log4net;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

IConfiguration configuration = builder.Configuration.GetSection("Redis") ?? throw new InvalidOperationException("Redis Connection string not found."); ;
string redisConfiguration = configuration["Configuration"];

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConfiguration)
);

//IConfiguration log4NetConfig = builder.Configuration.GetSection("log4net") ?? throw new InvalidOperationException("Log4Net configuration section not found."); ;
//string loggingConfiguration = log4NetConfig["Configuration"];

//builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(log4NetConfig));

builder.Services.AddLogging(builder =>
{
    builder.AddLog4Net("log4net.config");
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
