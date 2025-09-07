using Common.Contracts.Encryption;
using Logger.API.Features;
using Logger.Application.Services;
using Logger.Domain.Data;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var rabbitUri = new UriBuilder
{
    Scheme = builder.Configuration["MessageBroker:Protocol"],
    Host = builder.Configuration["MessageBroker:Host"],
    Port = int.Parse(builder.Configuration["MessageBroker:Port"]!),
    UserName = builder.Configuration["MessageBroker:User"],
    Password = builder.Configuration["MessageBroker:Password"],
    Path = builder.Configuration["MessageBroker:VirtualHost"]
}.Uri;

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ExceptionLogConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitUri);
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<MongoDbService>();
builder.Services.AddScoped<ExceptionLoggerService>();

builder.Services.AddSingleton<EncryptionHelper>();

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

app.Run();
