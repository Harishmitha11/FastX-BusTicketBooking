using Microsoft.EntityFrameworkCore;
using FastX_BusTicketBooking.API.Contexts;
using FastX_BusTicketBooking.API.Mappings;
using FastX_BusTicketBooking.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FastX_BusTicketBooking.API.Services.Interfaces;
using FastX_BusTicketBooking.API.Services.Implementations;
using Microsoft.OpenApi.Models;
using log4net;
using log4net.Config;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("FastXBusTicketBookingDB")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRefundService, RefundService>();



var logFilePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Logs", "FastX-BusTicketBooking.log");
GlobalContext.Properties["LogFileName"] = Path.GetFullPath(logFilePath);

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

builder.Services.AddSingleton<ILog>(LogManager.GetLogger(typeof(Program)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))

        };
    });

builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "jwt",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your valid JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>

{

    options.AddPolicy("AllowAll", builder =>

        builder.AllowAnyOrigin()

               .AllowAnyMethod()

               .AllowAnyHeader());

});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
