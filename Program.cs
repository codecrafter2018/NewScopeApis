

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Ultratechapis.Data;
using Ultratechapis.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<Connect>();
builder.Services.AddSingleton<OfferService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Offer Acceptance Rate Calculate API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Warm-up connection on startup
_ = app.Services.GetService<Connect>();

app.Run();
