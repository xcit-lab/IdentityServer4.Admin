using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services
{
    public class PersistedGrantAspNetIdentityService<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> 
		: IPersistedGrantAspNetIdentityService
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
        private readonly IPersistedGrantAspNetIdentityRepository _persistedGrantAspNetIdentityRepository;
        private readonly IPersistedGrantAspNetIdentityServiceResources _persistedGrantAspNetIdentityServiceResources;

        public PersistedGrantAspNetIdentityService(
			IPersistedGrantAspNetIdentityRepository persistedGrantAspNetIdentityRepository,
            IPersistedGrantAspNetIdentityServiceResources persistedGrantAspNetIdentityServiceResources)
        {
            _persistedGrantAspNetIdentityRepository = persistedGrantAspNetIdentityRepository;
            _persistedGrantAspNetIdentityServiceResources = persistedGrantAspNetIdentityServiceResources;
        }

        public async Task<PersistedGrantsDto> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _persistedGrantAspNetIdentityRepository.GetPersitedGrantsByUsers(search, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public async Task<PersistedGrantsDto> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var exists = await _persistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(subjectId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_persistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), _persistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

            var pagedList = await _persistedGrantAspNetIdentityRepository.GetPersitedGrantsByUser(subjectId, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public async Task<PersistedGrantDto> GetPersitedGrantAsync(string key)
        {
            var persistedGrant = await _persistedGrantAspNetIdentityRepository.GetPersitedGrantAsync(key);
            if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(_persistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), _persistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
            var persistedGrantDto = persistedGrant.ToModel();

            return persistedGrantDto;
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            return await _persistedGrantAspNetIdentityRepository.DeletePersistedGrantAsync(key);
        }

        public async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            return await _persistedGrantAspNetIdentityRepository.DeletePersistedGrantsAsync(userId);
        }
    }
}
