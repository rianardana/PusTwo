using Microsoft.EntityFrameworkCore;
using PusTwo.Application.DownTime.Interfaces;
using PusTwo.Application.Syspro.Interfaces;
using PusTwo.Infrastructure.Data;
using PusTwo.Infrastructure.Services;
using PusTwo.Web.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(WebMappingProfile).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("PusTwo.Infrastructure")));
        
builder.Services.AddDbContext<SysproDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SysproConnection"), 
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddScoped<ISysproService, SysproService>();
builder.Services.AddScoped<IDownTimeService, DownTimeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();