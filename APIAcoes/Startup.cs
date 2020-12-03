using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;
using APIAcoes.Models;
using APIAcoes.Validators;
using APIAcoes.Data;

namespace APIAcoes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<AcoesContext>(
                    options => options.UseSqlServer(
                        Configuration.GetConnectionString("BaseAcoesEF")));

            services.AddSingleton<ConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(
                    Configuration["Redis:Connection"]));

            services.AddControllers()
                .AddFluentValidation();

            services.AddTransient<IValidator<Acao>, AcaoValidator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIAcoes", Version = "v1" });
            });

            services.AddApplicationInsightsTelemetry(Configuration);
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
                (module, o) =>
                {
                    module.EnableSqlCommandTextInstrumentation = true;
                });

            services.AddAzureAppConfiguration();

            services.AddScoped<AcoesRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIAcoes v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(builder => builder.AllowAnyMethod()
                                          .AllowAnyOrigin()
                                          .AllowAnyHeader());

            app.UseAzureAppConfiguration();

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