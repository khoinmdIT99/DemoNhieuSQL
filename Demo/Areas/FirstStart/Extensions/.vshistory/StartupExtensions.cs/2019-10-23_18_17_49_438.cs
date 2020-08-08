using System;
using System.Threading.Tasks;

using Demo.Areas.FirstStart.Configurations;
using Demo.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Areas.FirstStart.Extensions
{
    public static class StartupExtensions
    {
        public static Func<Task> _RestartHost;

        public static IServiceCollection AddConfigureDatabaseWithIdentity(this IServiceCollection services, IConfiguration config)
        {
            string DataProvider = ConstStrings.DataProvider;

            Action<DbContextOptionsBuilder> optionsBuilder;

            switch (config[DataProvider].ToLowerInvariant())
            {
                case "sqlserver":
                    services.AddEntityFrameworkSqlServer();
                    optionsBuilder = options => options.UseSqlServer(config.GetConnectionString(ConstStrings.SqlServerConnection));
                    break;

                case "sqlite":
                    services.AddEntityFrameworkSqlite();
                    optionsBuilder = options => options.UseSqlite(config.GetConnectionString(ConstStrings.SQLiteConnection));
                    break;

                case "postgresql":
                    services.AddEntityFrameworkNpgsql();
                    optionsBuilder = options => options.UseNpgsql(config.GetConnectionString(ConstStrings.PostgreSQLConnection));
                    break;

                default:
                    services.AddEntityFrameworkInMemoryDatabase();
                    optionsBuilder = options => options.UseInMemoryDatabase(config.GetConnectionString(ConstStrings.InMemoryConnection));
                    break;
            }

            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                optionsBuilder(options);
                options.EnableSensitiveDataLogging();
            });

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IApplicationBuilder UseFirstStartConfiguration(this IApplicationBuilder app, IConfiguration configuration, Func<Task> restartHost)
        {
            _RestartHost = restartHost;

            if (configuration.GetValue<bool>(ConstStrings.IsFirstStart))
            {
                app.UseWhen(IsFirstStartIncomplete, configuration =>
                {
                    configuration.MapWhen(context => !IsStartsWithSegmentsFirstStart(context) && IsFirstStartIncomplete(context), configuration =>
                        configuration.Run(request =>
                        {
                            request.Response.Redirect(ConstStrings.FirstStart);
                            return Task.CompletedTask;
                        })
                    );

                    configuration.UseRouting();

                    configuration.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                    });
                });
            }

            return app;
        }

        private static bool IsFirstStartIncomplete(HttpContext context) =>
            context.RequestServices.GetRequiredService<IConfiguration>().GetValue<bool>(ConstStrings.IsFirstStart);

        private static bool IsStartsWithSegmentsFirstStart(HttpContext context) =>
            context.Request.Path.StartsWithSegments(ConstStrings.FirstStart);
    }
}
