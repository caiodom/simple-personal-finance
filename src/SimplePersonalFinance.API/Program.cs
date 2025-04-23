using SimplePersonalFinance.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddBuilderConfigurations();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();
app.UseConfigurations();
await app.RunAsync();
