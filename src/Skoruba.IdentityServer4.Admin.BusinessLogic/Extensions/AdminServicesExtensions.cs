using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Extensions
{
    public static class AdminServicesExtensions
    {
        public static IServiceCollection AddAdminServices<TAdminDbContext>(
            this IServiceCollection services)
            where TAdminDbContext : DbContext, IAdminPersistedGrantDbContext, IAdminConfigurationDbContext, IAdminLogDbContext
        {

            return services.AddAdminServices<TAdminDbContext, TAdminDbContext, TAdminDbContext>();
        }

        public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
        {
            //Repositories
            services.AddTransient<IClientRepository<TConfigurationDbContext>, ClientRepository<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceRepository<TConfigurationDbContext>, IdentityResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceRepository<TConfigurationDbContext>, ApiResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantRepository<TPersistedGrantDbContext>, PersistedGrantRepository<TPersistedGrantDbContext>>();
            services.AddTransient<ILogRepository, LogRepository<TLogDbContext>>();

            //Services
            services.AddTransient<IClientService, ClientService<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceService, ApiResourceService<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService<TPersistedGrantDbContext>>();
            services.AddTransient<ILogService, LogService<TLogDbContext>>();

            //Resources
            services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
            services.AddScoped<IClientServiceResources, ClientServiceResources>();
            services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
            services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();

            return services;
        }
    }
}
