using SimplePersonalFinance.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddBuilderConfigurations();

// Add services to the container.
builder.Services.AddServices(builder.Configuration);


var app = builder.Build();

app.UseConfigurations();

app.Run();
