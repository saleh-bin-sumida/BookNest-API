var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigurSwagger();
builder.Services.AddCoreServices();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new ArgumentNullException();
var connectionString = builder.Configuration.GetConnectionString("ProductionConnection") ?? throw new ArgumentNullException();

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddDataServices(connectionString);
builder.Services.AddAuthenticationServices(jwtOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookNest API v1");
    });

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });

    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}