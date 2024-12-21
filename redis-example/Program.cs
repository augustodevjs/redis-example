using System.Reflection;
using Microsoft.EntityFrameworkCore;
using redis_example.Abstractions.Caching;
using redis_example.Infrastructure.Caching;
using redis_example.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ToDoListDbContext>(o => 
        o.UseInMemoryDatabase("ToDoListDb"));

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddStackExchangeRedisCache(o => {
    o.InstanceName = "instance";
    o.Configuration = "localhost:6379";
});

builder.Services
    .AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();