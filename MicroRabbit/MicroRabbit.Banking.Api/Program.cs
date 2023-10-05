using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbitt.Banking.Domain.CommandHandlers;
using MicroRabbitt.Banking.Domain.Commands;
using MicroRabbitt.Banking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Add db context
builder.Services.AddDbContext<BankingDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banking Microservice",
        Version = "v1",
    });
});

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

//builder.Services.AddRazorPages();

//DEPENDECY CONTAINER
// Add services to the container.

//Domain Bus
builder.Services.AddTransient<IEventBus, RabbitMQBus>();

//Domain Banking Commands
builder.Services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

//Application Services
builder.Services.AddScoped<IAccountService, AccountService>();

//Data Layer
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
//builder.Services.AddTransient<BankingDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Microservice v1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

app.UseAuthorization();

app.MapControllers();

//app.MapRazorPages();

app.Run();
