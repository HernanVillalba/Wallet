using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wallet.Business.Profiles;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Business.Logic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Wallet.Entities;
using Wallet.Business;
using Microsoft.AspNetCore.Mvc;
using Wallet.Business.EmailSender.Interface;
using Wallet.Business.EmailSender;

namespace Wallet.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region ErrorHandling
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    // This code is executed when invalid ModelState occurs

                    ErrorModel error = new ErrorModel();
                    error.status = 400; // Set BadRequest error

                    // Map ModelState dictionary to Error dictionary
                    var dictionary = actionContext.ModelState;
                    error.errors = new Dictionary<string, List<string>>();
                    foreach(var validationError in dictionary)
                    {
                        List<string> errorList = new List<string>();
                        foreach (var innerError in validationError.Value.Errors)
                        {
                            errorList.Add(innerError.ErrorMessage);
                        }
                        error.errors.Add(validationError.Key != "" ? validationError.Key : "error", errorList);
                    }

                    return new BadRequestObjectResult(error);
                };
            });
            #endregion
            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
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
                            new string[] {}
                    }
                });
                var filePath = Path.Combine(AppContext.BaseDirectory, "Wallet.API.xml");
                c.IncludeXmlComments(filePath);
            });
            #endregion
            #region Database Context
            services.AddDbContext<WALLETContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WalletDB")));
            #endregion
            #region Repositories and Unit of Work
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IFixedTermDepositRepository, FixedTermDepositRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITransactionLogRepository, TransactionLogRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            #endregion
            #region AutoMapper
            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperProfile)));
            #endregion
            #region NewtonsoftJson
            services.AddControllers().AddNewtonsoftJson(options => options
                    .SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            #endregion
            #region JWT Authentication
            var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion
            #region Business Logic
            services.AddTransient<ISessionBusiness, SessionBusiness>();
            services.AddTransient<IAccountBusiness, AccountBusiness>();
            services.AddTransient<IUserBusiness, UserBusiness>();
            services.AddTransient<IFixedTermDepositBusiness, FixedTermDepositBusiness>();
            services.AddTransient<ITransactionBusiness, TransactionBusiness>();
            #endregion
            #region Mailer
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.Configure<SendGridEmailSenderOptions>(options =>
            {
                options.ApiKey = Configuration["ExternalProviders:SendGrid:ApiKey"];
                options.SenderEmail = Configuration["ExternalProviders:SendGrid:SenderEmail"];
                options.SenderName = Configuration["ExternalProviders:SendGrid:SenderName"];
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wallet.API v1"));
            }

            app.UseStatusCodePages(); // To handle responses without body

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseMiddleware<ExceptionHandler>(); // To handle the exceptions
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}