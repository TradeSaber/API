using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using TradeSaber.Authorization;
using TradeSaber.Services;
using TradeSaber.Settings;

namespace TradeSaber
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
            services.Configure<JWTSettings>(Configuration.GetSection(nameof(JWTSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);

            services.AddScoped<IAuthService, TradeSaberAuthService>();

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearerConfiguration(Configuration["JWTSettings:Issuer"], Configuration["JWTSettings:Audience"], Configuration["JWTSettings:Key"]);
            services.AddAuthorization(options => Array.ForEach(Scopes.AllScopes, scope => options.AddPolicy(scope, policy => policy.Requirements.Add(new ScopeRequirement(Configuration["JWTSettings:Issuer"], scope)))));
            services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}