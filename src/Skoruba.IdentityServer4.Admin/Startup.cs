using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Extensions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Extensions;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;

namespace Skoruba.IdentityServer4.Admin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
			services.ConfigureRootConfiguration(Configuration);
			var rootConfiguration = services.BuildServiceProvider().GetService<IRootConfiguration>();

			/// BEFORE: Adds a single data context including the identity context (which enforces the user type).
			//services.AddDbContexts<AdminDbContext>(HostingEnvironment, Configuration);
			///
			/// AFTER: This adds all data contexts that can be administered, except for the identity context (see below).
			services.AddDbContexts<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(HostingEnvironment, Configuration);

			/// BEFORE: - (new)
			//
			/// AFTER: Adds the identity context (mapping to my own application user types).
			services.AddDbContext<MyApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey),
					sql => sql.MigrationsAssembly(typeof(MyApplicationDbContext).Assembly.GetName().Name)));

			/// BEFORE: Adds authentication into the UI, against the single data context and enforcing the user types.
			//services.AddAuthenticationServices<AdminDbContext, UserIdentity, UserIdentityRole>(HostingEnvironment, rootConfiguration.AdminConfiguration);
			///
			/// AFTER: Adds authentication into the UI, against my own application context and user types.
			services.AddAuthenticationServices<MyApplicationDbContext, MyApplicationUser, MyIdentityRole>(HostingEnvironment, rootConfiguration.AdminConfiguration);

			/// BEFORE == AFTER. No change.
			services.AddMvcExceptionFilters();

			/// BEFORE: Adds services that support the UI, binding into the single data context.
			//services.AddAdminServices<AdminDbContext>();
			///
			/// AFTER: Adds services that support the UI, binding into separated data contexts.
			services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

			/// BEFORE: Adds services to modify identity resources, tapping into the single data context and enforced user types.
			//services.AddAdminAspNetIdentityServices<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
			//                    UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole,
			//                    UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>();
			///
			/// AFTER: Adds services to modify identity resources from a separated data context and against own user type.
			services.AddAdminAspNetIdentityServices<MyApplicationDbContext, IdentityServerPersistedGrantDbContext, MyApplicationUser>();

			/// BEFORE == AFTER. No change.
			services.AddMvcLocalization();
			services.AddAuthorizationPolicies();
		}

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.AddLogging(loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSecurityHeaders();
            app.UseStaticFiles();
            app.ConfigureAuthentificationServices(env);
            app.ConfigureLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}