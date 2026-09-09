using MicroERP.API.Middlewares;
using MicroERP.Application;
using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Providers;
using MicroERP.Application.Common.Files;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using MicroERP.Infrastructure;
using MicroERP.Infrastructure.Authorization;
using MicroERP.Infrastructure.Identity;
using MicroERP.Infrastructure.Services.Files;
using MicroERP.Infrastructure.Settings;
using MicroERP.Persistence;
using MicroERP.Persistence.Authorization;
using MicroERP.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);



#region Database

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<FileValidationSettings>(
    builder.Configuration.GetSection("FileValidation"));

#endregion




#region Application + Infrastructure

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
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

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });


    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                    new OpenApiReference
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




#region Permission Providers

builder.Services.AddScoped<
    IMultiPermissionDefinitionProvider,
    HRDefinitionProvider>();


builder.Services.AddScoped<
    IMultiPermissionDefinitionProvider,
    RolePermissionDefinitionProvider>();


builder.Services.AddScoped<
    IMultiPermissionDefinitionProvider,
    IdentityDefinitionProvider>();


builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    EmployeeDefinitionProvider>();


builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    AuditDefinitionProvider>();


builder.Services.AddScoped<
    IPermissionDefinitionProvider,
    EmployeeDocumentsProvider>();

#endregion




#region Seeders

builder.Services.AddScoped<PermissionSeeder>();

#endregion

//QuestPDF.Settings.License = LicenseType.Evaluation;

QuestPDF.Settings.License = LicenseType.Community;
//builder.Services.AddHostedService<AttendanceDailyJob>();
//builder.Services.AddHostedService<AttendanceRecalculateJob>();
builder.Services.AddScoped<IAttendanceAbsentService, AttendanceAbsentService>();
//builder.Services.AddHostedService<AttendancePerformanceMonthlyJob>();
builder.Services.AddMemoryCache();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    //await context.Database.ExecuteSqlRawAsync(@"
    //        DELETE FROM Payrolls;
    //        DBCC CHECKIDENT ('Payrolls', RESEED, 0);
    //    ");

    //await context.Database.ExecuteSqlRawAsync(@"
    //        DELETE FROM PayrollItems;
    //        DBCC CHECKIDENT ('PayrollItems', RESEED, 0);
    //    ");

    //await context.Database.ExecuteSqlRawAsync(@"
    //    DELETE FROM AttendanceRecords;
    //    DBCC CHECKIDENT ('AttendanceRecords', RESEED, 0);
    //");


    //await context.Database.ExecuteSqlRawAsync(@"
    //    DELETE FROM AttendanceTransactions;
    //    DBCC CHECKIDENT ('AttendanceTransactions', RESEED, 0);
    //");
    //await context.Database.ExecuteSqlRawAsync(@"
    //    DELETE FROM AttendanceLogs;
    //    DBCC CHECKIDENT ('AttendanceLogs', RESEED, 0);
    //");

    //await context.Database.ExecuteSqlRawAsync(@"
    //        DELETE FROM PayrollAdjustments;
    //        DBCC CHECKIDENT ('PayrollAdjustments', RESEED, 0);
    //    ");

    //await context.Database.ExecuteSqlRawAsync(@"
    //    DELETE FROM EmployeeOvertimes;
    //    DBCC CHECKIDENT ('EmployeeOvertimes', RESEED, 0);
    //");

    //await context.Database.ExecuteSqlRawAsync(@"
    //   DELETE FROM AttendancePerformances;
    //    DBCC CHECKIDENT ('AttendancePerformances', RESEED, 0);
    //");

}







#region Database Seed

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;


    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();


    var roleManager =
        services.GetRequiredService<RoleManager<ApplicationRole>>();


    await IdentitySeeder.SeedAsync(
        userManager,
        roleManager);
    var seeder =
        scope.ServiceProvider
            .GetRequiredService<PermissionSeeder>();


    var providers =
        scope.ServiceProvider
            .GetServices<IPermissionDefinitionProvider>();
    var Multi =
        scope.ServiceProvider
            .GetServices<IMultiPermissionDefinitionProvider>();


    await seeder.SeedAsync(providers, Multi);
}



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // await AttendancePolicySeeder.SeedAsync(context);
     //await EmployeeEvaluationSeeder.SeedAsync(context);
   // await AttendanceLogSeeder.SeedAsync(context,app.Services);
    // await SalaryComponentSeeder.SeedAsync(context);
    // await EmployeeSalaryComponentSeeder.SeedAsync(context);
    // await PayrollAdjustmentSeeder.SeedAsync(context);
    //   await PayrollPolicySeeder.Seed(context);
     //  await EmployeeBankAccountSeeder.Seed(context);
}


#endregion




#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseAuthentication();

app.UseAuthorization();


app.UseMiddleware<ExceptionMiddleware>();


app.MapControllers();

#endregion


app.Run();