using MinimalApiBuilder;
using TodoApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = "Properties"
});

builder.AddSerilogLogger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMinimalApiBuilderEndpoints()
    .AddCors(corsOptions =>
    {
        corsOptions.AddDefaultPolicy(
            corsBuilder => { corsBuilder.WithOrigins(builder.Configuration.AllowedOrigins()); });
    });

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    context.Configuration.GetSection("Kestrel:Limits").Bind(serverOptions.Limits);
    context.ConfigureCertificate(serverOptions);
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

RouteGroupBuilder auxiliary = app.MapGroup("/").WithTags("Auxiliary");
auxiliary.MapGet("/health", () => TypedResults.Ok());

app.Run();
