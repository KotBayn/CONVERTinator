using CONVERTinator.Services;
using CONVERTinator.Repositories;
using CONVERTinator.Domain.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Allow all (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 
builder.Services.AddScoped<IDbRepository, DbRepository>();
builder.Services.AddScoped<ICacheSyncService, CacheSyncService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CONVERTinator.WebAPI.SyncWorker>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
// Enable the WebAPI to serve static front-end assets from the wwwroot directory
app.UseStaticFiles();

// Automatically apply the CORS policy to all endpoints
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();
app.Run();