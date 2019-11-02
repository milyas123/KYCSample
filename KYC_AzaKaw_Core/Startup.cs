using System; 
using KYC_AzaKaw_Core.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KYC_AzaKaw_Core.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using KYC_AzaKaw_Core.Extensions; 
using NLog;
using KYC_AzaKaw_Core.Helpers;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace KYC_AzaKaw_Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CustomerContext>(o => o.UseSqlServer(Configuration.GetConnectionString("CustomerDB")));
            services.AddSingleton<ILoggerManager, LoggerManager>(); 
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IUploadRepository, UploadRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
            options.TokenValidationParameters = new TokenValidationParameters
            {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, 
            ValidIssuer = "http://localhost:54016",
            ValidAudience = "http://localhost:54016",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
            };
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
          

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.ConfigureCustomExceptionMiddleware();
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();


        }
    }
}
