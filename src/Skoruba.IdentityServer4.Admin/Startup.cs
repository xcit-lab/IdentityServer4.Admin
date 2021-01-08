using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Shared.Helpers;

namespace Skoruba.IdentityServer4.Admin
{
	public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var rootConfiguration = CreateRootConfiguration();
            //services.AddSingleton(rootConfiguration);

            // Adds the IdentityServer4 Admin UI with custom options.
            services.AddIdentityServer4AdminUI(ConfigureUIOptions);

            // Add email senders which is currently setup for SendGrid and SMTP
            //services.AddEmailSenders(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
   //         app.UseCookiePolicy();

   //         if (env.IsDevelopment())
   //         {
   //             app.UseDeveloperExceptionPage();
   //         }
   //         else
   //         {
   //             app.UseExceptionHandler("/Home/Error");
   //             app.UseHsts();
   //         }

   //         app.UsePathBase(Configuration.GetValue<string>("BasePath"));

   //         // Add custom security headers
   //         app.UseSecurityHeaders(Configuration);

   //         app.UseStaticFiles();

   //         UseAuthentication(app);

			//// Use Localization
			//app.ConfigureLocalization();

			app.UseIdentityServer4AdminUI();

			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoint =>
            {
				endpoint.MapIdentityServer4AdminUI();

				//endpoint.MapDefaultControllerRoute();
				//endpoint.MapHealthChecks("/health", new HealthCheckOptions
				//{
				//	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				//});
			});
        }

        public virtual void ConfigureUIOptions(IdentityServer4AdminUIOptions options)
		{
            // Applies configuration from appsettings.
            options.ApplyConfiguration(Configuration);
            options.ApplyConfiguration(HostingEnvironment);
            
            // Use production DbContexts and auth services.
            options.IsStaging = false;
		}
    }
}