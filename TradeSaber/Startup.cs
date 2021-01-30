using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TradeSaber.Authorization;
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
            services.Configure<DiscordSettings>(_configuration.GetSection(nameof(DiscordSettings)));
            services.Configure<JWTSettings>(_configuration.GetSection(nameof(JWTSettings)));

            services.AddSingleton<IAuthService, DiscordAuthService>();
            services.AddSingleton<DiscordService>();

            services.AddHttpClient();
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearerConfiguration(_configuration["JWTSettings:Issuer"], _configuration["JWTSettings:Audience"], _configuration["JWTSettings:Key"]);
            services.AddAuthorization(options =>
            {
                var scopes = new[]
                {
                    Scopes.UploadFile
                };
                Array.ForEach(scopes, scope =>
                    options.AddPolicy(scope,
                        policy => policy.Requirements.Add(new ScopeRequirement(_configuration["JWTSettings:Issuer"], scope))));
            });

            services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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