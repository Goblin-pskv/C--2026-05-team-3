using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Infrastructure.Data;
using EventFlow.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(msc => msc.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<RegisterCommandHandler>();
builder.Services.AddScoped<UpdateProfileCommandHandler>();
var connectionString = builder.Configuration.GetConnectionString("LocalPostgres");

builder.Services.AddDbContext<EventFlowDbContext>(options =>
    options.UseNpgsql(connectionString));

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
