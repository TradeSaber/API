using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            services.Configure<MainSettings>(Configuration.GetSection(nameof(MainSettings)));
            services.Configure<SoriginSettings>(Configuration.GetSection(nameof(SoriginSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MainSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SoriginSettings>>().Value);

            services.AddHttpClient();
            services.AddSingleton<SoriginAuthorizer>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, TradeSaberAuthService>();

            services.AddDbContext<TradeContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Default"));
                options.UseSnakeCaseNamingConvention();
            });

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearerConfiguration(Configuration["JWTSettings:Issuer"], Configuration["JWTSettings:Audience"], Configuration["JWTSettings:Key"]);
            services.AddAuthorization(options => Array.ForEach(Scopes.AllScopes, scope => options.AddPolicy(scope, policy => policy.Requirements.Add(new ScopeRequirement(Configuration["JWTSettings:Issuer"], scope)))));
            services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TradeContext tradeContext, ILogger<Startup> logger)
        {
            logger.LogInformation("Ensuring that the database is created...");
            try
            {
                tradeContext.Database.Migrate();
            }
            catch
            {
                logger.LogInformation("Migrations were not applied.");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}