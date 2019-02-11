using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories
{
    public class PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> 
        : IPersistedGrantAspNetIdentityRepository
		where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
		where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
		where TUser : IdentityUser<TKey> 
        where TRole : IdentityRole<TKey> 
        where TKey : IEquatable<TKey> 
        where TUserClaim : IdentityUserClaim<TKey> 
        where TUserRole : IdentityUserRole<TKey> 
        where TUserLogin : IdentityUserLogin<TKey> 
        where TRoleClaim : IdentityRoleClaim<TKey> 
        where TUserToken : IdentityUserToken<TKey>
        
    {
        private readonly TIdentityDbContext _identityDbContext;
		private readonly TPersistedGrantDbContext _persistentGrantDbContext;

		public bool AutoSaveChanges { get; set; } = true;

        public PersistedGrantAspNetIdentityRepository(TIdentityDbContext identityDbContext, TPersistedGrantDbContext persistentGrantDbContext)
        {
			_identityDbContext = identityDbContext;
			_persistentGrantDbContext = persistentGrantDbContext;
		}

		public Task<PagedList<PersistedGrantDataView>> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10)
		{
			return Task<PagedList<PersistedGrantDataView>>.Run(() =>
			{
				var pagedList = new PagedList<PersistedGrantDataView>();

				var persistedGrantByUsers = (from pe in _persistentGrantDbContext.PersistedGrants.ToList()
											 join us in _identityDbContext.Users.ToList() on pe.SubjectId equals us.Id.ToString() into per
											 from us in per.DefaultIfEmpty()
											 select new PersistedGrantDataView
											 {
												 SubjectId = pe.SubjectId,
												 SubjectName = us == null ? string.Empty : us.UserName
											 })
											.Distinct();


				var persistedGrantsResult = persistedGrantByUsers;
				if (!string.IsNullOrEmpty(search))
				{
					Expression<Func<PersistedGrantDataView, bool>> searchCondition = x => x.SubjectId.Contains(search) || x.SubjectName.Contains(search);
					Func<PersistedGrantDataView, bool> searchPredicate = searchCondition.Compile();
					persistedGrantsResult = persistedGrantsResult.Where(searchPredicate);
				}

				var persistedGrantsData = persistedGrantsResult.AsQueryable().PageBy(x => x.SubjectId, page, pageSize).ToList();
				var persistedGrantsDataCount = persistedGrantsResult.Count();

				pagedList.Data.AddRange(persistedGrantsData);
				pagedList.TotalCount = persistedGrantsDataCount;
				pagedList.PageSize = pageSize;

				return pagedList;
			});
        }

        public async Task<PagedList<PersistedGrant>> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrant>();

            var persistedGrantsData = await _persistentGrantDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).Select(x => new PersistedGrant()
            {
                SubjectId = x.SubjectId,
                Type = x.Type,
                Key = x.Key,
                ClientId = x.ClientId,
                Data = x.Data,
                Expiration = x.Expiration,
                CreationTime = x.CreationTime
            }).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();

            var persistedGrantsCount = await _persistentGrantDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).CountAsync();

            pagedList.Data.AddRange(persistedGrantsData);
            pagedList.TotalCount = persistedGrantsCount;
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<PersistedGrant> GetPersitedGrantAsync(string key)
        {
            return _persistentGrantDbContext.PersistedGrants.SingleOrDefaultAsync(x => x.Key == key);
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            var persistedGrant = await _persistentGrantDbContext.PersistedGrants.Where(x => x.Key == key).SingleOrDefaultAsync();

			_persistentGrantDbContext.PersistedGrants.Remove(persistedGrant);

            return await AutoSaveChangesAsync();
        }

        public Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            return _persistentGrantDbContext.PersistedGrants.AnyAsync(x => x.SubjectId == subjectId);
        }

        public async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var grants = await _persistentGrantDbContext.PersistedGrants.Where(x => x.SubjectId == userId).ToListAsync();

			_persistentGrantDbContext.RemoveRange(grants);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _persistentGrantDbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public async Task<int> SaveAllChangesAsync()
        {
            return await _persistentGrantDbContext.SaveChangesAsync();
        }
    }
}