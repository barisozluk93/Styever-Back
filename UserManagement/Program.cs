using Microsoft.AspNetCore.HttpOverrides;
using UserManagement.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Authorization;
using UserManagement.BackgroundServices;
using UserManagement.DbContexts;
using UserManagement.Interfaces;
using UserManagement.Model;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var emailConfig = Configuration
        .GetSection("MailSettings")
        .Get<MailSettings>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddDbContext<UserManagementContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAgreementService, AgreementService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<ILegalContentService, LegalContentService>();
builder.Services.Configure<ShopierOptions>(Configuration.GetSection("Shopier"));
builder.Services.AddScoped<IShopierPaymentService, ShopierPaymentService>();
builder.Services.AddScoped<IPurchaseDocumentService, PurchaseDocumentService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddHostedService<UserDailyWorker>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Servis localhost'a bind olduğu ve public HTTPS reverse proxy'de sonlandığı için
    // proxy adresini önceden bilmiyorsak forwarded header'ları kabul ediyoruz.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
    scope.ServiceProvider.GetRequiredService<UserManagementContext>().Database.Migrate();
}

app.UseForwardedHeaders();

// Shopier isteğini MVC/action seçimine girmeden önce kaydet. Böylece 404/405/415,
// model-binding veya proxy kaynaklı bir sorun olsa bile isteğin servise ulaşıp ulaşmadığı görülür.
app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/api/User/ShopierOsb", StringComparison.OrdinalIgnoreCase))
    {
        ShopierFileLogger.Info(
            $"PIPELINE HIT. Method={context.Request.Method}, Scheme={context.Request.Scheme}, " +
            $"Host={context.Request.Host}, Path={context.Request.Path}, ContentType={context.Request.ContentType}, " +
            $"ContentLength={context.Request.ContentLength}, RemoteIp={context.Connection.RemoteIpAddress}");
    }

    await next();

    if (context.Request.Path.Equals("/api/User/ShopierOsb", StringComparison.OrdinalIgnoreCase))
        ShopierFileLogger.Info($"PIPELINE RESPONSE. StatusCode={context.Response.StatusCode}");
});

// HTTPS public reverse proxy'de sonlanıyor; uygulama sadece localhost:5224 dinliyor.
// Burada UseHttpsRedirection kullanmak, proxy X-Forwarded-Proto aktarmıyorsa callback'i
// controller'a ulaşmadan 307/308 ile geri çevirebilir.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:5224");
//app.Run();

