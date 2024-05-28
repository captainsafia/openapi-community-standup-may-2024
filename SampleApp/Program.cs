#define BASIC

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


#region CUSTOMINCLUSIONPREDICATE
#if CUSTOMINCLUSIONPREDICATE
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi("internal", options =>
    {
        options.ShouldInclude = (description) => description.RelativePath.Contains("internal");
    });

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/public", () => "Hello World!");
app.MapGet("/internal", () => "Hello Universe!");

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

#region SWAGGERUI
#if SWAGGERUI
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.MapGet("/", () => "Hello World!");

app.Run();
#endif
#endregion

#region CONSTRAINTSANDVALIDATIONS
#if CONSTRAINTSANDVALIDATIONS
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/hello/{name:length(6, 20)}", (string name) => $"Hello {name}");
app.MapPost("/todos", (AttributedTodo todo) => string.Empty);

app.Run();

internal class AttributedTodo
{
    [Required]
    [Range(1, 100)]
    public int Id { get; set; }
    [Required]
    [Description("A title for the todo item.")]
    public string Title { get; set; } = string.Empty;
    public bool Completed { get; set; }
}
#endif
#endregion

#region ENUMS
#if ENUMS
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/enum", (ClothingSize size) => TypedResults.Ok(size));
app.MapGet("/stringified-enum", (ClothingType clothingType) => TypedResults.Ok(clothingType));

app.Run();

[JsonConverter(typeof(JsonStringEnumConverter))]
enum ClothingType
{
    Shirt,
    Pants,
    Shoes
}

enum ClothingSize
{
    Small,
    Medium,
    Large

}
#endif
#endregion ENUMS

#region FORMS
#if FORMS

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapPost("/forms", (IFormFile image, [FromForm] Todo todo) => string.Empty);

app.Run();

internal record Todo(int Id, string Title, bool Completed);
#endif
#endregion

#region RECURSIVETYPES
#if RECURSIVETYPES

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapPost("/", (Proposal proposal) => string.Empty);

app.Run();

internal class Proposal
{
    public required Proposal ProposalElement { get; set; }
    public required Stream Stream { get; set; }
}
#endif
#endregion


#region POLYMORPHICTYPES
#if POLYMORPHICTYPES
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapPost("/polymorphic-types", (Shape shape) => string.Empty);

app.Run();


[JsonDerivedType(typeof(Triangle), typeDiscriminator: "triangle")]
[JsonDerivedType(typeof(Square), typeDiscriminator: "square")]
internal abstract class Shape
{
    public string Color { get; set; } = string.Empty;
    public int Sides { get; set; }
}

internal class Triangle : Shape
{
    public double Hypotenuse { get; set; }
}
internal class Square : Shape
{
    public double Area { get; set; }
}
#endif
#endregion