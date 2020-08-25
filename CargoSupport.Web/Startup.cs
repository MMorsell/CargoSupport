using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Microsoft.AspNetCore.Authentication.Negotiate;

using Microsoft.Extensions.DependencyInjection;

namespace CargoSupport.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();
            //TODO: Fix implementation of cookie auth
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //    });

            services.AddControllersWithViews();
            services.AddSignalR();

            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:32770",
                                                          "http://127.0.0.1:5500");
                                  });
            });

            //services.AddSingleton(provider => GetScheduler().Result);

            //services.AddAuthentication()
            //    .AddCookie(options => {
            //        options.LoginPath = "/Identity/Account/Login";
            //        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //    })
            //    .AddIdentityServerJwt();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private async Task<IScheduler> GetScheduler()
        {
            var properties = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", "BackgroundTasks.Web" },
                { "quartz.scheduler.instanceId", "BackgroundTasks.Web" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.useProperties", "true" },
                { "quartz.threadPool.threadCount", "1" },
                { "quartz.serializer.type", "json" },
            };
            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = await schedulerFactory.GetScheduler();

            var th = new CargoSupport.Web.Helpers.TaskHelper(scheduler);
            await th.CheckAvailability();
            //await scheduler.Start();
            return scheduler;
        }
    }
}