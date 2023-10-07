using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Interfaces;
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

//Add BANKING db context
builder.Services.AddDbContext<BankingDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
});

//Add TRANSFER db context
builder.Services.AddDbContext<TransferDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("TransferDbConnection"));
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
//builder.Services.AddSingleton<IEventBus, RabbitMQBus>();
builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp => 
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
});

//Subscriptions
builder.Services.AddTransient<TransferEventHandler>();

//Domain Banking Commands
builder.Services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

//Domain TRANSFER Commands
builder.Services.AddTransient<IEventHandler<MicroRabbitt.Transfer.Domain.Events.TransferCreatedEvent>, TransferEventHandler>();

//Application Services
builder.Services.AddScoped<IAccountService, AccountService>();

//Data Layer
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
//builder.Services.AddTransient<BankingDbContext>();

//AQUI OS DE TRANSFER

//Application Services
builder.Services.AddScoped<ITransferService, MicroRabbit.Transfer.Application.Services.TransferService>();

//Data Respository Layer
builder.Services.AddScoped<ITransferRepository, TransferRepository>();

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

ConfigureEventBus(app);

void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<MicroRabbitt.Transfer.Domain.Events.TransferCreatedEvent, TransferEventHandler>();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

app.UseAuthorization();

app.MapControllers();

//app.MapRazorPages();

app.Run();
