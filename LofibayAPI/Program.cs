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
DotNetEnv.Env.Load();
builder.Services.AddControllers();

//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LofibayDev")));
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_LOFIBAY_DEV")!));
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LofibayStaging")));
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_LOFIBAY_STAGING")!));
//builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LofibayProduction")));
builder.Services.AddDbContext<LofibayDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_LOFIBAY_PRODUCTION")!));

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPhotoService, PhotoService>();
builder.Services.AddTransient<ICollectionService, CollectionService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL")));

string myCorsPolicy = "MyCorsPolicy";
builder.Services.AddCors(p => p.AddPolicy(myCorsPolicy, builder =>
{
    builder.WithOrigins(Environment.GetEnvironmentVariable("MY_ORIGIN")!).AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Token-Expired");
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
        options.Audience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
            ClockSkew = TimeSpan.Zero, // default is five minutes (not sure)
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors(myCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
