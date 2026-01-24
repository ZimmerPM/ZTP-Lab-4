using ProductCart.BFF.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var productApiUrl = builder.Configuration["ExternalApis:ProductApi"]
    ?? throw new InvalidOperationException("ProductApi URL not configured");
var cartApiUrl = builder.Configuration["ExternalApis:CartApi"]
    ?? throw new InvalidOperationException("CartApi URL not configured");

builder.Services.AddInfrastructure(productApiUrl, cartApiUrl);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
