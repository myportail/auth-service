using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        }

        public IConfiguration Configuration { get; }
        public Settings.Application AppSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            var connection = @"server=localhost;database=auth;user=sa;password=igQFUwjZZyxgken7gcKg*gTu";
            
            services.AddDbContext<Contexts.UsersDbContext>(options => options.UseSqlServer(connection));
            services.AddScoped<Services.IUsersService, Services.UsersService>();
            services.AddScoped<Services.IAuthService, Services.AuthService>();
            services.AddScoped<Services.IPasswordHasher, Services.PasswordHasher>();
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Contexts.UsersDbContext>();
                context.Database.Migrate();
            }

            app.UseSwagger();
            
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            
//            app.UseSwaggerUI(c =>
//            {
//                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
//            });
            
            app.UseMvc();
        }
    }
}
