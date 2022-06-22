using Orders.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Repositories;
using Orders.Infra.Repositories;
using Autofac;
using MediatR;
using Serilog;
using Autofac.Extensions.DependencyInjection;
using Orders.Api.Controllers;
using Orders.App.DomainEventHandlers;
using BuildingBlocks.EventBusRabbitMQ;
using RabbitMQ.Client;
using Orders.Api.AutofacModules;
using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.EventBus;
using Orders.App.IntegrationEvents;
using Orders.App.Services;
using Orders.Api.ErrorHandling;
using Orders.App.Mappers;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new MediatorModule()));

// Add services to the container.

ConfigureServices(builder.Services);

AddCustomIntegrations(builder.Services, configuration);

//-------------- logging ---------------------
//added: Configure JSON logging to the console.
builder.Logging.AddJsonConsole();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//-------------- Postgres configuration ----------
var postgresConnectionString = builder.Configuration["PostgreSqlConnectionString"];

builder.Services.AddDbContext<OrdersDbContext>(opt =>
{
    opt.UseNpgsql(postgresConnectionString);
    opt.EnableSensitiveDataLogging(true);
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

app.ConfigureCustomExceptionMiddleware();

InitDatabase();

app.Run();


/////////
IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen();

    //custom
    services.AddMediatR(typeof(OrderCreatedDomainEventHandler).Assembly);

    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
    {
        var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        var retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
        {
            retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
        }

        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
    });

    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
}

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl, null)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

void AddCustomIntegrations(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    services.AddSingleton<IHttpStatusCodeMapper, HttpStatusCodeMapper>();

    services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

    services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

    services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

    services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

        var factory = new ConnectionFactory();
        factory.Uri = new Uri(configuration["EventBusEndPoint"]);

        var retryCount = 5;
        if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
        {
            retryCount = int.Parse(configuration["EventBusRetryCount"]);
        }

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    });
}

void InitDatabase()
{
    //initialize database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<OrdersDbContext>();
            DbInitializer.InitializeContent(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating the database.");
        }
    }
}

public partial class Program
{
    public static string Namespace = typeof(OrdersController).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}

