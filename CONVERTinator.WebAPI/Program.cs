using CONVERTinator.Services;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CONVERTinator.WebAPI.SyncWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
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