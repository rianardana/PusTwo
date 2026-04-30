using Microsoft.EntityFrameworkCore;
using PusTwo.Application.DTOs.Syspro;
using PusTwo.Application.Interfaces;
using PusTwo.Infrastructure.Data;
using PusTwo.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(Program)); 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("PusTwo.Infrastructure")));
builder.Services.AddDbContext<SysproDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SysproConnection"), 
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddScoped<ISysproService, SysproService>();
builder.Services.AddScoped<IDownTimeService, DownTimeService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
