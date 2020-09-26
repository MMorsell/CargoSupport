using System.Threading.Tasks;
using System;
using CargoSupport.Helpers;
using CargoSupport.Hubs;
using CargoSupport.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using CargoSupport.Models.Auth;
using CargoSupport.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CargoSupport.Web.IIS
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
            // Add db services and auth.
            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<MongoDbSettings>(settings);

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(options =>
            //        {
            //            options.LoginPath = "/Account/Login/"; // auth redirect
            //            options.ExpireTimeSpan = new TimeSpan(0, 0, 0, 20);
            //        });

            services.AddIdentity<ApplicationUser, MongoIdentityRole>()
                    .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(settings.ConnectionString, settings.DatabaseName)
                    .AddSignInManager()
                    .AddRoleManager<RoleManager<MongoIdentityRole>>()
                    .AddDefaultTokenProviders();

            //Dependency Injection and services
            services.AddSingleton<IMongoDbService, MongoDbService>();
            services.AddHostedService<PinUpdateService>();
            services.AddScoped<IDataConversionHelper, DataConversionHelper>();
            services.AddScoped<IQuinyxHelper, QuinyxHelper>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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

            app.UseDeveloperExceptionPage();

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
                    pattern: "{controller=Account}/{action=Home}/{id?}");
            });

            var RoleManager = serviceProvider.GetRequiredService<RoleManager<MongoIdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            VerifyRolesAndSuperUserExist(UserManager, RoleManager).Wait();
        }

        private async Task VerifyRolesAndSuperUserExist(UserManager<ApplicationUser> _userManager, RoleManager<MongoIdentityRole> _roleManager)
        {
            foreach (var role in Constants.MinRoleLevel.AllRoles)
            {
                bool roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new MongoIdentityRole { Name = role });
                }
            }

            var user = new ApplicationUser { UserName = "Superuser@live.se", Email = "Superuser@live.se" };

            var result = await _userManager.CreateAsync(user, "TodoPassword.123");
            if (result.Succeeded)
            {
                var currentUser = await _userManager.FindByNameAsync(user.UserName);

                await _userManager.AddToRoleAsync(currentUser, Constants.MinRoleLevel.SuperUserAndUp);
            }
            else
            {
                //TODO: Update function to reset superuser password, and better handling if user already exists in database, i.e fake error
                //throw new Exception(result.Errors.ToString());
            }
        }
    }
}