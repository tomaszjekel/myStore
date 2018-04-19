using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Framework;
using MyStore.Infrastructure;
using MyStore.Infrastructure.EF;
using MyStore.Services;

namespace MyStore
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
            var appOptions = new AppOptions();
            var section = Configuration.GetSection("app");
            section.Bind(appOptions);

            services.AddMvc()
                .AddCookieTempDataProvider()
                .AddSessionStateTempDataProvider();
            services.Configure<AppOptions>(Configuration.GetSection("app"));
            services.AddSingleton(ctx => ctx.GetService<IOptions<AppOptions>>().Value);
            services.AddSingleton(appOptions);
            services.AddResponseCaching();
            services.AddSession();
            services.AddScoped<IProductRepository, EfProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAuthenticator, Authenticator>();
            services.AddSingleton(AutoMapperConfig.GetMapper());
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<MyStoreContext>();
            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/account/login");
                    o.AccessDeniedPath = new PathString("/forbidden");
                    o.ExpireTimeSpan = TimeSpan.FromDays(1);
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

//            using (var serviceScope = app.ApplicationServices
//                .GetService<IServiceScopeFactory>().CreateScope())
//            {
//                var context = serviceScope.ServiceProvider.GetService<MyStoreContext>();
//                context.Database.Migrate(); 
//            }

            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            // app.Use(async (ctx, next) =>
            // {
            //     //Console.WriteLine($"Path: {ctx.Request.Path.ToString()}");
            //     Console.WriteLine("Before next");
            //     await next();
            //     Console.WriteLine("After next");
            // });

            // app.Run(async ctx => Console.WriteLine("Run")); 

            //app.UseMiddleware(typeof(ErrorHandlerMiddleware));
            app.UseMiddleware<ErrorHandlerMiddleware>();         

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
