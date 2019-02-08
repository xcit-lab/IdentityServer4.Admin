using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts
{
	public class IdentityServerPersistedGrantDbContext : PersistedGrantDbContext, IAdminPersistedGrantDbContext
	{
		public IdentityServerPersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions) 
			: base(options, storeOptions)
		{
		}
	}
}
