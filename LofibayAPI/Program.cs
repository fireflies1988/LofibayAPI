using CloudinaryDotNet;
using Common.Helpers;
using DataAccessEF;
using DataAccessEF.Services;
using DataAccessEF.UnitOfWork;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.Initialize(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Lofibay")));
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LofibayDev")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPhotoService, PhotoService>();
builder.Services.AddTransient<ICollectionService, CollectionService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(new Cloudinary(builder.Configuration!["CloudinaryUrl"]));
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "LofibayAPI", Version = "v1" });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (\"bearer [accessToken]\").",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["Jwt:ValidAudience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            ClockSkew = TimeSpan.Zero, // default is five minutes (not sure)
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration!["Jwt:SecretKey"]))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
