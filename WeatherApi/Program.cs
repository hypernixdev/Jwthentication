using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        //x.Authority = "https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_F4Ka3zB68";
        //x.MetadataAddress =
        //    "https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_F4Ka3zB68/.well-known/openid-configuration";
        //x.IncludeErrorDetails = true;
        //x.RequireHttpsMetadata = false;
        //x.TokenValidationParameters = new TokenValidationParameters
        //{
        //    ValidateIssuerSigningKey = true,
        //    ValidateLifetime = true,
        //    ValidateIssuer = true,
        //    ValidateAudience = false
        //};

        // JWT from IdentityAPI
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey("HypernixForTheLoveOfGodStoreAndLoadThisSecurely"u8.ToArray()),
            ValidIssuer = "https://id.hypernix.com",
            ValidAudience = "https://hypernix.com",
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weather", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .RequireAuthorization()
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
