
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ECommerceAPI.Domain.Interfaces;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Application.Mappings;
using ECommerceAPI.Infrastructure.UnitOfWork;
using ECommerceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;

namespace ECommerceAPI.API
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddControllers(options =>
            {
               
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

        
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

       
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString); 
                                                        
                options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
                options.LogTo(Console.WriteLine, LogLevel.Information);
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Dependency Injection
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IImageService, ImageService>();

            // JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key not found in configuration.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce Inventory API",
                    Version = "v1",
                    Description = "A RESTful API for e-commerce inventory management with image upload support",
                    Contact = new OpenApiContact
                    {
                        Name = "Your Name",
                        Email = "your.email@example.com"
                    }
                });

                // Enable file upload support in Swagger UI
                c.OperationFilter<FileUploadOperationFilter>();

                // Try to include XML comments
                try
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                catch
                {
                    // Ignore if XML comments file is not found
                }

                // JWT Authentication in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce Inventory API v1");
                    
                });
            }

            app.UseHttpsRedirection();
            
            // Enable static file serving for images
            app.UseStaticFiles();
            
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Attempting to create database...");

                    // Test connection
                    if (await context.Database.CanConnectAsync()) 
                    {
                        logger.LogInformation("Database connection successful");
                    }
                    else
                    {
                        logger.LogWarning("Cannot connect to database");
                    }

                    // Create database if it doesn't exist
                    await context.Database.EnsureCreatedAsync();
                    logger.LogInformation("Database created successfully");
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while creating the database");
                throw; 
            }

            await app.RunAsync(); 
        }
    }
}
