using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Framework;
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
                .AddSessionStateTempDataProvider()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.Configure<AppOptions>(Configuration.GetSection("app"));
            services.AddSingleton(ctx => ctx.GetService<IOptions<AppOptions>>().Value);
            services.AddSingleton(appOptions);
            services.AddResponseCaching();
            services.AddSession();
            services.AddLocalization(options => options.ResourcesPath = "Resources");


            services.AddScoped<IProductRepository, EfProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartProvider, CartProvider>();


            services.AddScoped<IFileRepository, EfFileRepository>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAuthenticator, Authenticator>();
            services.AddSingleton(AutoMapperConfig.GetMapper());

            var connMySql = "server=localhost;port=3306;uid=root;password=Blokersi123$;database=MyStore;";
            services.AddDbContext<MyStoreContext>(options => options.UseMySql(connMySql));

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

            app.UseResponseCaching();

            app.UseSession();
            app.UseAuthentication();
            app.UseStaticFiles(); // For the wwwroot folder
            app.UseFileServer(new FileServerOptions
            {

                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("UPLOAD_DIR"))),
                RequestPath = "/images",
                EnableDirectoryBrowsing = true
            });

            //app.UseMiddleware(typeof(ErrorHandlerMiddleware));
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseStaticFiles();

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("pl-PL"),
                };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pl-PL"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
