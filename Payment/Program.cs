using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payment.MessageBroker;
using Payment.Persistences;
using Payment.Services;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
builder.Services.AddHandlerService();

// Message Broker
builder.Services.AddMassTransit(mt =>
{
    // Auth
    mt.AddConsumer<NewUserConsumer>();
    // Ticket
    mt.AddConsumer<CreatePaymentConsumer>();
    mt.AddConsumer<CancelPaymentConsumer>();
    mt.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(5));
        cfg.ConfigureEndpoints(ctx);
        cfg.Host(builder.Configuration.GetSection("RabbitMQ").GetSection("HostName").Value, host =>
        {
            host.Username(builder.Configuration.GetSection("RabbitMQ").GetSection("UserName").Value);
            host.Password(builder.Configuration.GetSection("RabbitMQ").GetSection("Password").Value);
        });
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Assesment API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter token here",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();