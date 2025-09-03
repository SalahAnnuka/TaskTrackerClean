using Hangfire;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskTrackerClean.API.Middlewares;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Application.Services;
using TaskTrackerClean.Domain.Data;
using TaskTrackerClean.Domain.Interfaces;
using TaskTrackerClean.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/errors.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .Enrich.FromLogContext()
    .MinimumLevel.Error()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


var rabbitUri = new UriBuilder
{
    Scheme = builder.Configuration["DatabaseSettings:RabbitMQ:Protocol"],
    Host = builder.Configuration["DatabaseSettings:RabbitMQ:Host"],
    Port = int.Parse(builder.Configuration["DatabaseSettings:RabbitMQ:Port"]!),
    UserName = builder.Configuration["DatabaseSettings:RabbitMQ:User"],
    Password = builder.Configuration["DatabaseSettings:RabbitMQ:Password"],
    Path = builder.Configuration["DatabaseSettings:RabbitMQ:VirtualHost"]
}.Uri;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitUri);
        cfg.ConfigureEndpoints(context);
    });
});

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var sqlBuilder = new SqlConnectionStringBuilder
{
    DataSource = builder.Configuration["DatabaseSettings:SqlServer:Host"],
    InitialCatalog = builder.Configuration["DatabaseSettings:SqlServer:Database"],
    IntegratedSecurity = bool.Parse(builder.Configuration["DatabaseSettings:SqlServer:TrustedConnection"]!)
};

var sqlConnection = sqlBuilder.ConnectionString;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        sqlConnection,
        b => b.MigrationsAssembly("TaskTrackerClean.Infrastructure")
    ));



builder.Services.AddHangfire(config => {
    var hangfireBuilder = new SqlConnectionStringBuilder
    {
        DataSource = builder.Configuration["DatabaseSettings:Hangfire:Host"],
        InitialCatalog = builder.Configuration["DatabaseSettings:Hangfire:Database"],
        IntegratedSecurity = bool.Parse(builder.Configuration["DatabaseSettings:Hangfire:TrustedConnection"]!)
    };
    var hangfireConnection = hangfireBuilder.ConnectionString;

    config.UseSqlServerStorage(hangfireConnection);
});

builder.Services.AddHangfireServer();


builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskReportRepository, TaskReportRepository>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskReportService, TaskReportService>();
builder.Services.AddScoped<ReportSchedulerService>();
builder.Services.AddScoped<MongoDbService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var reportSchedulerService = scope.ServiceProvider.GetRequiredService<ReportSchedulerService>();
    reportSchedulerService.ConfigureDailyReportJob();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();