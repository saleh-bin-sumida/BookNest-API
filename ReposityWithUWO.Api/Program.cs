using Microsoft.EntityFrameworkCore;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUWO.EF;
using RepositoryWithUWO.EF.Repositories;
using RepositoryWithUOW.Core.AutoMapperProfiles;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var Constr = builder.Configuration.GetSection("ConnectionsStrings:DefaultConnections").Value;

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(Constr,
b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

//builder.Services.AddTransient(typeof(IBaseRepository<>),typeof(BaseRepository<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>( );
builder.Services.AddAutoMapper(typeof(ProfileMapper).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
