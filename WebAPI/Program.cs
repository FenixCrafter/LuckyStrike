using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Handlers;

var builder = WebApplication.CreateBuilder(args);
var imageStorageLocation = builder.Configuration.GetValue<string>("StorageLocation");
// if (!Directory.Exists(imageStorageLocation))
// {
//     Directory.CreateDirectory(imageStorageLocation);
// }

// Add services to the container.

var mailPassword = builder.Configuration.GetValue<string>("Mail:Password");
var mail = builder.Configuration.GetValue<string>("Mail:Email");

var smtp = new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    Credentials = new NetworkCredential( mail, mailPassword),
    EnableSsl = true,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    UseDefaultCredentials = false,
};

builder.Services.AddSingleton(smtp);

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApiDocument();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        builder =>
        {
            builder.
             AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<RoleHandler>();
builder.Services.AddScoped<FileHandler>();
builder.Services.AddScoped<OrderHandler>();
builder.Services.AddScoped<DateHandler>();
builder.Services.AddScoped<CodeHandler>();
builder.Services.AddScoped<PdfHandler>();
builder.Services
            .AddDefaultIdentity<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentityServer()
.AddDeveloperSigningCredential()
    .AddApiAuthorization<User, ApplicationDbContext>();

var key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Secret"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = (context) =>
            {
                string token = context.HttpContext.Request.Headers.Authorization;
                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Split(' ')[1];
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, imageStorageLocation)),
    // FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, imageStorageLocation)),
    RequestPath = "/FileStorage",
    OnPrepareResponse = (context) =>
    {
        context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    }
});

await app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleHandler>().ValidateRoles();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();
 
app.MapControllers();

app.Run();
