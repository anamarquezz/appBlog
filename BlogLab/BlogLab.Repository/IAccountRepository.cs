using BlogLab.Models.Account;
using Microsoft.AspNet.Identity;

namespace BlogLab.Repository
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancelationToken);
        public Task<ApplicationUserIdentity> GetByUsernameAsync(string normalizedUsername,
            CancellationToken cancelationToken);
    }
}