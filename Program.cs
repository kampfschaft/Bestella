using Bestella.Components;
using Bestella.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbPath = "App_Data/orders.db";
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

Console.WriteLine("Using database path: " + Path.GetFullPath(dbPath));

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddAntiforgery();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("Environment: " + app.Environment.EnvironmentName);

app.Run();
