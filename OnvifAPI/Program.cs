using EFOnvifAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnvifAPI.Interfaces;
using OnvifAPI.Repository;
using OnvifAPI.Service;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddDbContext<masterContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "OpenCorsPolicy",
                      builder =>
                      {
                          builder
                            .WithOrigins("http://localhost:3000")
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});


builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
});
builder.Services.AddScoped<DbContext, masterContext>();

builder.Services.AddTransient(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));
builder.Services.AddTransient<ICameraFrameTimeService, CameraFrameTimeService>();
builder.Services.AddTransient<ICameraService, CameraService>();
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailAndWhatsSenderService, EmailAndWhatsAppSenderService>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("OpenCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
