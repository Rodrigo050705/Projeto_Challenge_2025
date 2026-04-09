
using Mgb.DependencyInjections.DependencyInjectons.Extensions;
using Mgb.ServiceBus.ServiceBus.Extensions;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.RegisterAllDependencies();
builder.Services.RegisterServiceBus(builder.Configuration);


builder.Services.AddSingleton<ChatClient>(serviceProvider =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    var model = "gpt-5-nano";

    return new ChatClient(model, apiKey);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("dev");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();