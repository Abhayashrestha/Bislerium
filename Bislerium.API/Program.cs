using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Bislerium.API.Respositories.Implementation;
using Bislerium.API.Respositories.Repository;
using Bislerium.API.Respositories.Respository;
using FluentEmail.Smtp;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AuthDbcontext>(option =>
{
    option.UseSqlite(builder.Configuration.GetConnectionString("BisleriumConnection"));
});

/*builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlite(builder.Configuration.GetConnectionString("BisleriumConnection"));
});*/



builder.Services.AddScoped<ITokenRespository, TokenRespository>();
builder.Services.AddScoped<IEmailServiceRespository, EmailServiceRespositoy>();


builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Bislerium")
    .AddEntityFrameworkStores<AuthDbcontext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            AuthenticationType = "Jwt",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


var smtpConfig = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
var emailSender = new SmtpSender(() => new SmtpClient(smtpConfig.Host)
{
    UseDefaultCredentials = false,
    Port = smtpConfig.Port,
    Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password),
    EnableSsl = smtpConfig.EnableSsl,
});


/*builder.Services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));*/
builder.Services.AddScoped<IBlogRepository, BlogRespository>();
builder.Services.AddScoped<IReactionRespository, ReactionRespository>();
builder.Services.AddScoped<IEmailServiceRespository,EmailServiceRespositoy>();
builder.Services.AddScoped<ICommentRespository,CommentRespository>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MemoryBufferThreshold = Int32.MaxValue;
    options.ValueLengthLimit = Int32.MaxValue;
    options.MultipartBodyLengthLimit = Int32.MaxValue; // Adjust the size as necessary
    options.MultipartHeadersLengthLimit = Int32.MaxValue;
});

var emailServices = builder.Services.AddFluentEmail(smtpConfig.Username);
emailServices.AddSmtpSender(() => new SmtpClient
{
    Host = smtpConfig.Host,
    Port = smtpConfig.Port,
    EnableSsl = smtpConfig.EnableSsl,
    Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password),
    UseDefaultCredentials = false
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("MyAllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
