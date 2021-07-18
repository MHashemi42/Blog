using Blog.Data;
using Blog.Data.Entities;
using Blog.Data.Identity;
using Blog.Data.Repositories;
using Blog.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                {
                    policy.RequireAuthenticatedUser()
                        .RequireRole("Admin");
                });
            });

            services.AddDbContext<BlogDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<BlogDbContext>()
                .AddErrorDescriber<PersianErrorDescriber>()
                .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(config =>
            {
                config.TokenLifespan = TimeSpan.FromHours(1);
            });
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                ApplicationUserClaimsPrincipalFactory>();

            services.Configure<MailKitSettings>(Configuration.GetSection("MailKitSettings"));
            services.Configure<ReCaptchaSettings>(Configuration.GetSection("ReCaptcha"));

            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddHttpClient<ICaptchaService, ReCaptchaService>(c =>
            {
                c.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<ApplicationRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            IdentityDataInitializer.SeedData(userManager, roleManager, Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id:int?}");

                endpoints.MapRazorPages();
            });
        }
    }
}
