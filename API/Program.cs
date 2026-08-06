using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusiness(builder.Configuration);
builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
await app.Services.SeedDataAsync();

app.UseHttpsRedirection();

app.UseCors("AllowWeb");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
