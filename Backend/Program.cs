using Microsoft.Extensions.Options;
using URLShortener.Data;
using StackExchange.Redis;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbContext>();

// Cấu hình Redis
var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton(redis.GetDatabase());

// Đăng ký dịch vụ URL Shortener
builder.Services.AddScoped<URLShortener.Services.UrlService>();

// Thêm Controller & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
