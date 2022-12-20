using MinimalApiBuilder;
using Web.Extensions;
using Web.Features.Admin.Login;
using Web.Features.Customer.Create;
using Web.Features.Customer.List;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints();

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
admin.MapPut<AdminLogin>("/login");

RouteGroupBuilder customers = app.MapGroup("/customers").WithTags("Customers");
customers.MapGet<CustomerCreate>("/create");
customers.MapPost<CustomerList>("/list");

app.Run();
