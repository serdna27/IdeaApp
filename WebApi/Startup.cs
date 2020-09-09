using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdeaApp.Models.Repo;
using IdeaApp.Utils;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;

namespace IdeaApp
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<IdeaDbContext>();
            services.AddScoped<IUserRepository>(x=>
            new UserRepository(x.GetRequiredService<IdeaDbContext>(),_logger));
            // (opt=>opt.UseSqlite(@"Data Source=app.db"));

            
            // For Identity  
            services.AddIdentity<User,IdentityRole<int>>()
                .AddEntityFrameworkStores<IdeaDbContext>()
                .AddDefaultTokenProviders();



            services.AddCors(c =>
            {
                c.AddPolicy("AllowAllOrigins", options => {
                    // options.AllowAnyOrigin();
                    options.WithOrigins("*").WithHeaders("*")
                    .AllowAnyMethod();
                    // options.AllowAnyHeader();
                    // options.AllowAnyMethod();
                }
                    );
            });

              // Note .AddMiniProfiler() returns a IMiniProfilerBuilder for easy intellisense
            services.AddMiniProfiler(options =>
            {
                // All of this is optional. You can simply call .AddMiniProfiler() for all defaults
                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

            }).AddEntityFramework();

            services.AddMvcCore()
            .AddApiExplorer();

            services.AddSwaggerGen(c=>{
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            // app.UseCors(options => options.WithOrigins("http://localhost:8081").AllowAnyMethod());
            _logger = logger;
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");


                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept, Authorization" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                // context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                await next();
            });

            if (env.IsDevelopment())
            {      app.UseMiniProfiler();
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

               app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            app.UseAuthorization();

            
            
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });


        

        }
    }
}
