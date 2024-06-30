namespace Api;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Api.Attributes;
using Api.Configs;
using Api.Extensions;
using Dal;
using Dal.Configs;
using Dal.Interfaces;
using Dal.ServiceApi;
using EfCoreRepository.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MlkPwgen;
using Models.Models;
using Newtonsoft.Json;
using Scrutor;
using static Dal.Utilities.ConnectionStringUtility;

public class Startup
{
    private readonly IWebHostEnvironment _env;

    private readonly IConfigurationRoot _configuration;

    public Startup(IWebHostEnvironment env)
    {
        _env = env;

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpsRedirection(options => options.HttpsPort = 443);

        // If environment is localhost, then enable CORS policy, otherwise no cross-origin access
        services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder
            .WithOrigins(_configuration.GetSection("TrustedSpaUrls").Get<string[]>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        // Add framework services
        // Add functionality to inject IOptions<T>
        services.AddOptions();

        services.AddResponseCompression();

        services.Configure<JwtSettings>(_configuration.GetSection("JwtSettings"));

        services.AddLogging();

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddDistributedMemoryCache();

        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "dotnet-template", Version = "v1", Description = "Basic .NET Core template", License =
                    new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://github.com/amir734jj/dotnet-template.git"),
                    },
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                config.IncludeXmlComments(xmlPath);
            }

            config.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    }, new List<string>()
                },
            });
        });

        services.AddMvc(x => x.Filters.Add<ExceptionFilterAttribute>()).AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddRazorPagesOptions(x => x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

        services.Scan(scan => scan
            .FromAssemblies(Assembly.Load("API"), Assembly.Load("Logic"), Assembly.Load("DAL"))
            .AddClasses() // to register
            .UsingRegistrationStrategy(RegistrationStrategy.Skip) // 2. Define how to handle duplicates
            .AsImplementedInterfaces() // 2. Specify which services they are registered as
            .WithTransientLifetime()); // 3. Set the lifetime for the services

        services.AddDbContext<EntityDbContext>(opt =>
        {
            var postgresConnectionString =
                ConnectionStringUrlToPgResource(_configuration.GetRequiredValue<string>("DATABASE_URL"));
            opt.UseNpgsql(postgresConnectionString);
        });

        services.AddIdentity<User, IdentityRole<int>>(x => x.User.RequireUniqueEmail = false)
            .AddEntityFrameworkStores<EntityDbContext>()
            .AddRoles<IdentityRole<int>>()
            .AddDefaultTokenProviders();

        var jwtSetting = _configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>();

        if (_env.IsDevelopment() && string.IsNullOrEmpty(jwtSetting.Key))
        {
            jwtSetting.Key = PasswordGenerator.Generate(length: 100, allowed: Sets.Alphanumerics);

            IdentityModelEventSource.ShowPII = true;
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;

                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key)),
                };
            });

        services.AddEfRepository<EntityDbContext>(x => x.Profile(Assembly.Load("Dal")));

        services.AddSingleton(jwtSetting);

        var (accessKeyId, secretAccessKey, url) = (
            _configuration.GetRequiredValue<string>("CLOUDCUBE_ACCESS_KEY_ID"),
            _configuration.GetRequiredValue<string>("CLOUDCUBE_SECRET_ACCESS_KEY"),
            _configuration.GetRequiredValue<string>("CLOUDCUBE_URL"));

        var prefix = new Uri(url).Segments.GetValue(1)?.ToString();
        const string bucketName = "cloud-cube";

        // Generally bad practice
        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);

        // Create S3 client
        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(credentials, RegionEndpoint.USEast1));
        services.AddSingleton(new S3ServiceConfig(bucketName, prefix));

        services.AddSingleton<IFileService>(ctx => new S3FileService(
            ctx.GetRequiredService<ILogger<S3FileService>>(),
            ctx.GetRequiredService<IAmazonS3>(),
            ctx.GetRequiredService<S3ServiceConfig>()));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCors("CorsPolicy");

        app.UseResponseCompression();

        if (_env.IsDevelopment())
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        }

        // Not necessary for this workshop but useful when running behind kubernetes
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
            // This is particularly important so that HttpContent.Request.Scheme will be correct behind a SSL terminating proxy
            ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto,
        });

        // Use wwwroot folder as default static path
        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseCors("CorsPolicy")
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        Console.WriteLine("Application Started!");
    }
}