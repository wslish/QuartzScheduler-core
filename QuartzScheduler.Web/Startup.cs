using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuartzScheduler.Infrastructure.Models;
using QuartzScheduler.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl;
using Quartz;
using Quartz.Spi;
using System.Collections.Specialized;

namespace QuartzScheduler.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionStr = Configuration.GetConnectionString("MySqlConnection");

            services.AddDbContext<TaskDbContext>(options =>
              options.UseMySql(connectionStr));

            services.AddSingleton<JobScheduler>();
            services.AddTransient<HttpRequestJob>();

            var schedulerFactory = new StdSchedulerFactory();

            var scheduler = schedulerFactory.GetScheduler().Result;
            services.AddSingleton<IJobFactory, SimpleInjectorJobFactory>();
            services.AddSingleton<ISchedulerFactory>(schedulerFactory);
            services.AddSingleton(scheduler);

            services.AddMvc();

            var builder = services.BuildServiceProvider();
            scheduler.JobFactory = builder.GetService<IJobFactory>();

            return builder;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            Task.Delay(1000 * 10).ContinueWith(async (t) =>
            {
                var scheduler = serviceProvider.GetService<JobScheduler>();

                await scheduler.Init();
            });
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Task}/{action=Index}/{id?}");
            });


        }
    }
}
