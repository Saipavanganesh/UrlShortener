using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin() // Allows all origins
                .AllowAnyHeader()  // Allows all headers
                .AllowAnyMethod(); // Allows all methods
        });
});
// Add services to the container.
builder.Services.AddDbContext<UrlShortenerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UrlShortenerDb")));
builder.Services.AddSingleton<SnowflakeIdGenerator>(provider =>
{
    var machineId = builder.Configuration.GetValue<int>("SnowflakeConfig:MachineId");
    return new SnowflakeIdGenerator(machineId);
});
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.Run();
