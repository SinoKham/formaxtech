using Formaxtech.WebAPI.Middlewares;
using Formaxtech.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MapService>();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Наш middleware (важно: ДО app.UseAuthorization())
app.UseMiddleware<RequestLimitMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();