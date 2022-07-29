using AutoMapper;
using DataAccessEF;
using DataAccessEF.UnitOfWork;
using Domain.Interfaces;
using LofibayAPI.Mappings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Lofibay")));
builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LofibayDev")));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
