using Logger.API.Features;
using Logger.Application.Services;
using Logger.Domain.Data;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ExceptionLogConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["MessageBroker:Uri"]!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<MongoDbService>();
builder.Services.AddScoped<ExceptionLoggerService>();

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
