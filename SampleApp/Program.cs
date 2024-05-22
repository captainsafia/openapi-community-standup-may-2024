#define MULTIDOC

#region BASIC
#if BASIC
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/", () => "Hello World!");

app.Run();
#endif
#endregion

#region MULTIDOC
#if MULTIDOC
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi("internal")
    .AddOpenApi("public");

var app = builder.Build();

app.MapOpenApi();

var publicApis = app.MapGroup("/public")
    .WithGroupName("public");
var internalApis = app.MapGroup("/internal")
    .WithGroupName("internal");

internalApis.MapGet("/", () => "Hello World!");
publicApis.MapGet("/", () => "Hello Universe!");

app.Run();
#endif
#endregion