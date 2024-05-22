using Microsoft.AspNetCore.Authentication.JwtBearer;
using TwitchLib.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7211";
        options.Audience = "StreamWorks";
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<TwitchAPI>(new TwitchAPI());

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Twitch", client =>
{
    client.BaseAddress = new Uri("https://api.twitch.tv/helix/");
    //client.DefaultRequestHeaders.Add("Client-ID", "YOUR_CLIENT");
});

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

app.Run();
