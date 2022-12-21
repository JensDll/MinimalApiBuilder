using MinimalApiBuilder;
using Web.Extensions;
using Web.Features.Admin.Login;
using Web.Features.Customer.Create;
using Web.Features.Customer.List;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMinimalApiBuilderEndpoints();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthentication();
// app.UseAuthorization();

RouteGroupBuilder admin = app.MapGroup("/admin").WithTags("Admin");
admin.MapPost<AdminLogin>("/login");
admin.MapPatch<AdminLogin>("/login");

RouteGroupBuilder customers = app.MapGroup("/customer").WithTags("Customer");
customers.MapPost<CustomerCreate>("/create");
customers.MapGet<CustomerList>("/list");

app.Run();
