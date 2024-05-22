#define SCALAR

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

#region TRANSFORMERS
#if TRANSFORMERS
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddJwtBearer();

builder.Services.AddOpenApi(options =>
{
    options.UseTransformer(new InfoTransformer());
    options.UseOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Summary = "Transformed Summary";
        return Task.CompletedTask;
    });
    options.UseTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/", () => "Hello World!");

app.Run();

class InfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Contact = new OpenApiContact
        {
            Name = "API Author",
            Email = "iloveapis@example.com"
        };
        return Task.CompletedTask;
    }
}

class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
    }
}
#endif
#endregion

#region SCALAR
#if SCALAR
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Hello World!");

app.Run();
#endif
#endregion

#region SCHEMAS
#if SCHEMAS

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/stringified-enum", (ClothingType clothingType) => TypedResults.Ok(clothingType));
app.MapGet("/forms", (IFormFile resume) => string.Empty);


app.Run();

[JsonConverter(typeof(JsonStringEnumConverter))]
enum ClothingType
{
    Shirt,
    Pants,
    Shoes
}
#endif
#endregion