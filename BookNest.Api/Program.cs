using Microsoft.EntityFrameworkCore;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUWO.EF;
using RepositoryWithUWO.EF.Repositories;
using RepositoryWithUOW.Core.AutoMapperProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RepositoryWithUWO.Api;
using RepositoryWithUOW.Core.Services;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUWO.Core.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);


// register EF
var Constr = builder.Configuration.GetSection("ConnectionsStrings:DefaultConnections").Value;
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(Constr,
b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));



// register Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{   
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 5;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



// register objects
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>( );
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddAutoMapper(typeof(ProfileMapper).Assembly);



// register authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issure,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Book Nest API V1");
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
