using System.Reflection;
using System.Text;
using EventFlow.Application.Behaviors;
using EventFlow.Application.Commands.EventCommands;
using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.RegistrationCommands;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Application.Queries.EventQueries;
using EventFlow.Application.Queries.GetProfileQuery;
using EventFlow.Application.Queries.LoginQuery;
using EventFlow.Application.Queries.RegistrationQueries;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using EventFlow.Infrastructure.Repositories;
using EventFlow.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logsConnectionString = builder.Configuration.GetConnectionString("LogsConnection");

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()

        .WriteTo.PostgreSQL(
            connectionString: logsConnectionString,
            tableName: "logs",
            needAutoCreateTable: true,
            columnOptions: new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter() },
                { "message_template", new MessageTemplateColumnWriter() },
                { "level", new LevelColumnWriter() },
                { "time_stamp", new TimestampColumnWriter() },
                { "exception", new ExceptionColumnWriter() },
                { "properties", new PropertiesColumnWriter() }
            }
        );
});


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event API",
        Version = "v1",
        Description = "API для системы регистрации на мероприятия",
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    //Xml комментарии для сваггера
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    // НАСТРОЙКА JWT АВТОРИЗАЦИИ
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите JWT токен в формате: Bearer {ваш_токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<UpdateProfileCommandHandler>();
builder.Services.AddScoped<GetProfileQueryHandler>();
builder.Services.AddScoped<LoginQueryHandler>();

builder.Services.AddScoped(typeof(IEventRepository), typeof(EventRepository));

builder.Services.AddScoped<CreateEventCommandHandler>();
builder.Services.AddScoped<UpdateEventCommandHandleMock>();
builder.Services.AddScoped<CancelRegistrationCommandHandleMock>();
builder.Services.AddScoped<CreateRegistrationCommandHandleMock>();
builder.Services.AddScoped<GetEventQueryHandleMock>();
builder.Services.AddScoped<GetEventRegistrationsQueryHandleMock>();
builder.Services.AddScoped<GetUserRegistrationsQueryHandleMock>();
builder.Services.AddScoped<JwtSettings>();
builder.Services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<EventFlowDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped(typeof(ITokenService), typeof(TokenService));
builder.Services.AddScoped(typeof(IRefreshTokenService), typeof(RefreshTokenService));
builder.Services.AddScoped<IValidationService, ValidationService>();
// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "Секция 'JwtSettings' не найдена или не может быть корректно привязана в конфигурации.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<EventFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgres")));

builder.Services.AddProblemDetails(); // обработка ошибок

var app = builder.Build();

app.UseSerilogRequestLogging(); // логируем все http запросы
// сервис создания ролей
using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await DataSeeder.SeedRolesAsync(roleManager, userManager);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Ошибка при создании ролей");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
