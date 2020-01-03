using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using authService.Contexts;
using authService.Model.Api;
using authService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
//using MySql.Data.EntityFrameworkCore.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace authService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppSettings = Configuration.GetSection("App").Get<Settings.Application>();

            AppSettings.Validate();
        }

        public IConfiguration Configuration { get; }
        public Settings.Application AppSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            
            var dbConnString = $"Server=138.197.130.242;Database=dev_authusers;Uid=authservice;Pwd=R*Yd&K98M^f#@+U#";
            
            services.AddDbContext<UserContext>(options => options.UseMySql(dbConnString));
            services.AddScoped<Services.IUsersService, Services.UsersService>();
            services.AddScoped<Services.IAuthService, Services.AuthService>();
            services.AddScoped<Services.IPasswordHasher, Services.PasswordHasher>();
            services.AddScoped<Services.IMongoDbService, Services.StubMongoDbService>();
            services.AddSingleton<Settings.Application>(AppSettings);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = AppSettings.TokenGeneration.Issuer,
                        ValidAudience = AppSettings.TokenGeneration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(AppSettings.TokenGeneration.SecurityKey))
                    };
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                SetupDefaultUser(app).Wait();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseSwagger();
                app.UseAuthentication();

                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseMvc();
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
                throw ex;
            }
        }

        async Task SetupDefaultUser(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var mongoDbService = serviceScope.ServiceProvider.GetService<Services.IMongoDbService>();
                await mongoDbService.Init();

//                var context = serviceScope.ServiceProvider.GetService<Contexts.UsersDbContext>();
//                context.Database.Migrate();

                var userService = serviceScope.ServiceProvider.GetService<Services.IUsersService>();
                var adminUser = await userService.GetUserByName("Admin");
                if (adminUser == null)
                {
                    await userService.AddUser(new User()
                    {
                        Name = "Admin",
                        Password = "Admin@123"
                    });
                }
            }            
        }
    }
}
