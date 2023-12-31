using BlazorApp1.Data;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Versions.Compatibility;
using Orleans.Versions.Selector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.UseSignalR(); // Adds ability #1 and #2 to Orleans.
    siloBuilder.Configure<GrainVersioningOptions>(options =>
    {
        options.DefaultCompatibilityStrategy = nameof(StrictVersionCompatible);
        options.DefaultVersionSelectorStrategy = nameof(LatestVersion);
    });
    siloBuilder.UseDashboard(x => x.HostSelf = true);
});

builder.Services.AddSignalR().AddOrleans();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.Map("/dashboard", x => x.UseOrleansDashboard());
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();
