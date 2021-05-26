using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using System;
using System.IO;
using TradeSaber.Authorization;
using TradeSaber.Processors;
using TradeSaber.Services;
using TradeSaber.Settings;

namespace TradeSaber
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var deploymentSettings = _configuration.GetSection(nameof(DeploymentSettings)).Get<DeploymentSettings>();
            services.Configure<DiscordSettings>(_configuration.GetSection(nameof(DiscordSettings)));
            services.Configure<HTISettings>(_configuration.GetSection(nameof(HTISettings)));
            services.Configure<JWTSettings>(_configuration.GetSection(nameof(JWTSettings)));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DiscordSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HTISettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);

            services.AddScoped<IAuthService, DiscordAuthService>();
            services.AddScoped<ObjectiveIconService>();
            services.AddSingleton<DiscordService>();
            services.AddScoped<CardDispatcher>();
            services.AddScoped<RewardService>();
            services.AddScoped<MascotService>();
            services.AddSingleton<HTILoader>();
            services.AddScoped<TradeService>();
            services.AddScoped<HTIService>();
            services.AddSingleton<Random>();
            services.AddDbContext<TradeContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("Default"));
                options.UseSnakeCaseNamingConvention();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "_allowTSOrigins", opt =>
                {
                    opt.WithOrigins(deploymentSettings.CORS)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddHttpClient();
            services.AddControllers();

            services
                .AddImageSharp()
                .ClearProcessors()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<SimpleReizeWebProcessor>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearerConfiguration(_configuration["JWTSettings:Issuer"], _configuration["JWTSettings:Audience"], _configuration["JWTSettings:Key"]);
            services.AddAuthorization(options =>
            {
                Array.ForEach(Scopes.AllScopes, scope =>
                    options.AddPolicy(scope,
                        policy => policy.Requirements.Add(new ScopeRequirement(_configuration["JWTSettings:Issuer"], scope))));
            });

            services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TradeContext tradeContext)
        {
            tradeContext.Database.EnsureCreated();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(error =>
                {
                    error.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("A server error has occured!");
                    });
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("_allowTSOrigins");

            app.UseDefaultFiles();
            app.UseImageSharp();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallback(async context => await context.Response.WriteAsync("Not Found"));
                endpoints.Map("/", async context => await context.Response.WriteAsync("TradeSaber OK"));
                endpoints.Map("/api", async context => await context.Response.WriteAsync("TradeSaber API OK"));
            });
        }
    }
}