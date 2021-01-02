using System;
using NodaTime;
using System.IO;
using System.Net.Http;
using System.Reflection;
using TradeSaber.Settings;
using TradeSaber.Services;
using TradeSaber.Authorization;
using Microsoft.OpenApi.Models;
using SixLabors.ImageSharp.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Providers;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Middleware;
using NodaTime.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;

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
            services.Configure<RaritySettings>(Configuration.GetSection(nameof(RaritySettings)));
            services.Configure<JWTSettings>(Configuration.GetSection(nameof(JWTSettings)));
            services.Configure<HTISettings>(Configuration.GetSection(nameof(HTISettings)));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DiscordSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RaritySettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HTISettings>>().Value);

            services.AddSingleton<Random>();
            services.AddHttpContextAccessor();
            services.AddSingleton<HttpClient>();
            services.AddTransient<CardDispatcher>();
            services.AddSingleton<DiscordService>();
            services.AddSingleton<IClock>(SystemClock.Instance);
            
            services.AddDbContext<TradeContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(Configuration.GetConnectionString("Default"), o => o.UseNodaTime());
                optionsBuilder.UseSnakeCaseNamingConvention();
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(opt =>
                {
                    opt.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddImageSharp()
            .SetRequestParser<QueryCollectionRequestParser>()
            .Configure<PhysicalFileSystemCacheOptions>(options =>
            {
                options.CacheFolder = "image-cache";
            })
            .SetCache(provider =>
            {
                return new PhysicalFileSystemCache(
                            provider.GetRequiredService<IOptions<PhysicalFileSystemCacheOptions>>(),
                            provider.GetRequiredService<IWebHostEnvironment>(),
                            provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>(),
                            provider.GetRequiredService<FormatUtilities>());
            })
            .SetCacheHash<CacheHash>()
            .AddProvider<PhysicalFileSystemProvider>()
            .AddProcessor<ResizeWebProcessor>()
            .AddProcessor<FormatWebProcessor>()
            .AddProcessor<BackgroundColorWebProcessor>()
            .AddProcessor<JpegQualityWebProcessor>();

            services.AddControllers().AddJsonOptions(c => c.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Bcl));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TradeSaber", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TradeContext tradeContext, ILogger<Startup> logger)
        {
            //var path = Path.Combine(env.ContentRootPath, "wwwroot");
            //env.WebRootPath = path;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TradeSaber v1"));
            }

            logger.LogInformation("Ensuring Database Connection...");
            tradeContext.Database.EnsureCreated();
            app.UseHttpsRedirection();

            //Directory.CreateDirectory(path);
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(path),
            //    RequestPath = "/wwwroot"
            //});

            app.UseDefaultFiles();
            app.UseImageSharp();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseImageSharp();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware<JWTValidator>();

            logger.LogInformation("Mapping Endpoints...");
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