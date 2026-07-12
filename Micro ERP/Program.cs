using Domin.Entities;
using MicroERP.API.Middlewares;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Providers;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Auth.Interfaces;
using MicroERP.Application.Features.Departments.Interfaces;
using MicroERP.Application.Features.Departments.Services;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Application.Features.Employees.Services;
using MicroERP.Application.Features.Permissions.Interfaces;
using MicroERP.Application.Features.Roles.Interfaces;
using MicroERP.Application.Features.UserPermissions.Interfaces;
using MicroERP.Domain.Identity;
using MicroERP.Infrastructure.Authorization;
using MicroERP.Infrastructure.Identity;
using MicroERP.Infrastructure.Security;
using MicroERP.Infrastructure.Services;
using MicroERP.Infrastructure.Settings;
using MicroERP.Persistence;
using MicroERP.Persistence.Authorization;
using MicroERP.Persistence.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);



#region Database

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    options.EnableSensitiveDataLogging();
});


builder.Services.AddPersistence(
    builder.Configuration);

#endregion




#region Identity + Authentication

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(10);
});



builder.Services.Configure<JwtSettings>(
    builder.Configuration
        .GetSection("JwtSettings"));


var jwtSettings =
    builder.Configuration
        .GetSection("JwtSettings")
        .Get<JwtSettings>();



builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
    });

#endregion




#region Authorization

builder.Services.AddAuthorization();


builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();


builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

#endregion




#region MVC + Swagger

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
            Array.Empty<string>()
        }
    });
});

#endregion




#region Application Services


builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IdentityService, dentityService>();
builder.Services.AddScoped<ICredentialGenerator,CredentialsGenerator>();
builder.Services.AddScoped<IApplicationDbContext,ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IAuthorizationManager,AuthorizationManager>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserRoleService,UserRoleService>();
builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IUserPermissionAssignmentService,UserPermissionAssignmentService>();
builder.Services.AddScoped<IAuditService,AuditService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

#endregion




#region Permission Providers

builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    HRDefinitionProvider>();

builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    RolePermissionDefinitionProvider>();

builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    IdentityDefinitionProvider>();

builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    EmployeeDefinitionProvider>();

builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    AuditDefinitionProvider>();

#endregion




#region Seeders

builder.Services.AddScoped<
    PermissionSeeder>();

#endregion





var app = builder.Build();




#region Database Seed


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;


    var userManager =
        services.GetRequiredService<
            UserManager<ApplicationUser>>();


    var roleManager =
        services.GetRequiredService<
            RoleManager<ApplicationRole>>();



    await IdentitySeeder.SeedAsync(
        userManager,
        roleManager);
}



using (var scope = app.Services.CreateScope())
{
    var seeder =
        scope.ServiceProvider
            .GetRequiredService<PermissionSeeder>();


    var providers =
        scope.ServiceProvider
            .GetServices<IPermissionDefinitionProvider>();


    await seeder.SeedAsync(providers);
}

#endregion





#region Middleware


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();


app.UseMiddleware<ExceptionMiddleware>();


app.MapControllers();


#endregion



app.Run();