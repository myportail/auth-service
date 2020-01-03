using System;
using System.Threading;
using System.Threading.Tasks;
using authInit.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace authInit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Configuration.Application AppConfiguration
        {
            get { return this.Configuration.GetSection("App").Get<Configuration.Application>(); }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appConfig = AppConfiguration;
            var dbConnString = appConfig.AuthDb.ConnectionString;
            
            services.AddDbContext<UserContext>(options => options.UseMySql(dbConnString));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            TryUpdateDb(app).Wait();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        async Task TryUpdateDb(IApplicationBuilder app)
        {
            var updateSucceeded = false;
            do
            {
                Console.WriteLine("Attemping to update database");
                updateSucceeded = await UpdateDb(app);
                if (!updateSucceeded)
                {
                    Console.WriteLine("database update failed : waiting 30sec to retry");
                    await Task.Delay(30 * 1000);
                }
            } while (!updateSucceeded);
            
            Console.WriteLine("database update successful");
        }
        
        async Task<Boolean> UpdateDb(IApplicationBuilder app)
        {
            try
            {
                using var serviceScope = app.ApplicationServices.CreateScope();
                var context = serviceScope.ServiceProvider.GetService<UserContext>();

                await context.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
