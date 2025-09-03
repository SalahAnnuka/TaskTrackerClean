using Hangfire;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskTrackerClean.API.JobSchedulers;
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

////Database Registry

//App database/fetch connection string
var sqlBuilder = new SqlConnectionStringBuilder
{
    DataSource = builder.Configuration["DatabaseSettings:SqlServer:Host"],
    InitialCatalog = builder.Configuration["DatabaseSettings:SqlServer:Database"],
    IntegratedSecurity = bool.Parse(builder.Configuration["DatabaseSettings:SqlServer:TrustedConnection"]!)
};

//App database/initialize connection
var sqlConnection = sqlBuilder.ConnectionString;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        sqlConnection,
        b => b.MigrationsAssembly("TaskTrackerClean.Infrastructure")
    ));

//Hangfire
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

////DI Registry

//Repos
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskReportRepository, TaskReportRepository>();

//Services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskReportService, TaskReportService>();
builder.Services.AddScoped<MongoDbService>();

//Job Schedulers
builder.Services.AddTransient<ReportScheduler>();

//// Apply Registries
var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Run Schedulers
using (var scope = app.Services.CreateScope())
{
    var scheduler = scope.ServiceProvider.GetRequiredService<ReportScheduler>();
    scheduler.ConfigureDailyReportJob();
}


//add dev mode portals
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

//add exception handling middleware (logs errors)
app.UseMiddleware<ExceptionHandlingMiddleware>();

//reject https requests
app.Use(async (context, next) =>
{
    if (context.Request.IsHttps)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("HTTP required.");
        return;
    }

    await next();
});

app.UseRouting();
app.MapControllers();

app.Run();