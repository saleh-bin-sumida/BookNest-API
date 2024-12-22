/// <summary>
/// Entry point for the application.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adds controllers to the services collection.
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Adds API explorer for endpoint discovery.
/// </summary>
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Registers core services.
/// </summary>
builder.Services.RegisterCoreServices();

/// <summary>
/// Adds a scoped service for unit of work.
/// </summary>
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

/// <summary>
/// Configures JSON options to ignore loop references in models.
/// </summary>
builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

/// <summary>
/// Configures Swagger for API documentation.
/// </summary>
builder.Services.AddSwaggerGen(c =>
{
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiDescriptionGroupCollectionProvider>();
    var endpointCount = provider.ApiDescriptionGroups.Items.SelectMany(group => group.Items).Count();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookNest API Documentation",
        Description = $"Here Are {endpointCount} Endpoints",
        Version = "v1 Last Update 2024-12-22"
    });

    // Optional: Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

/// <summary>
/// Retrieves JWT options from configuration.
/// </summary>
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddSingleton(jwtOptions);

/// <summary>
/// Registers Entity Framework with SQL Server.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("ProductionConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

/// <summary>
/// Configures Identity services.
/// </summary>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Configures authentication with JWT bearer tokens.
/// </summary>
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