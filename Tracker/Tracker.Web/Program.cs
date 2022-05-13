using Microsoft.EntityFrameworkCore;
using Tracker.Business;
using Tracker.Core.Services;
using Tracker.Core.Services.Abstractions;
using Tracker.DataLayer;
using Tracker.DataLayer.Entities;
using Tracker.Web;
using Tracker.Web.Extensions;
using Tracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilogLogging();
// Add services to the container.

var services = builder.Services;
services.AddControllersWithViews()
       .AddRazorRuntimeCompilation();
services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

services.AddIdentity<ApplicationUser, ApplicationRole>(
    options =>
    {
        options.Password.RequiredLength = 7;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
  .AddEntityFrameworkStores<DataContext>();

services.AddScoped<IUserContextService, UserContextService>();

//services.AddHttpContextAccessor();
services.AddAutoMapper(typeof(AutoMappingProfile));
//Core Services
services.AddSingleton<IPasswordGeneratorService, PasswordGeneratorService>();
//Add Repository and Unit Of Work Dependencies 
services.ConfigureRepositoriesServices();
//Add Manager Dependencies            
services.ConfigureBusinessManagerServices((IConfiguration)builder.Configuration);


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
  );
});

logger.LogInformation("Start Migrate");
app.Migrate(logger);
logger.LogInformation("Complete Migrate");

logger.LogInformation("Start Setup InitializeDatabase");
app.InitializeDatabase(logger);
logger.LogInformation("Complete Setup InitializeDatabase");
app.Run();
