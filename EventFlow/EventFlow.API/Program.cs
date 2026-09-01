using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Application.Queries.GetProfileQuery;
using EventFlow.Application.Queries.LoginQuery;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using EventFlow.Infrastructure.Repositories;
using EventFlow.Infrastructure.Services;
using FluentValidation;
using EventFlow.Application.Behaviors;
using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(msc => msc.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<UpdateProfileCommandHandler>();
builder.Services.AddScoped<GetProfileQueryHandler>();
builder.Services.AddScoped<LoginQueryHandler>();
builder.Services.AddScoped<JwtSettings>();
builder.Services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<EventFlowDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped(typeof(ITokenService), typeof(TokenService));
builder.Services.AddScoped(typeof(IRefreshTokenService), typeof(RefreshTokenService));
builder.Services.AddScoped<IValidationService, ValidationService>();
// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<EventFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgres")));

var app = builder.Build();

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
        Console.WriteLine($"Ошибка при создании ролей: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
    }
}

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
