using System.Net.Http;
using TradeSaber.Settings;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TradeSaber
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
            services.Configure<DiscordSettings>(Configuration.GetSection(nameof(DiscordSettings)));
            services.Configure<JWTSettings>(Configuration.GetSection(nameof(JWTSettings)));
            services.Configure<HTISettings>(Configuration.GetSection(nameof(HTISettings)));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DiscordSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HTISettings>>().Value);

            services.AddSingleton<HttpClient>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(opt =>
                {
                    opt.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("TradeSaber OK!");
                });
                endpoints.MapGet("/api", async context =>
                {
                    await context.Response.WriteAsync("TradeSaber OK!");
                });
                endpoints.MapControllers();
                endpoints.MapFallback(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Not Found");
                });
            });
        }
    }
}