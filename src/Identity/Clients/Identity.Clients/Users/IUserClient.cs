using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Clients.Users
{
    public interface IUserClient
    {
        public Task<IReadOnlyCollection<UserSummaryClientResponse>> GetAllAsync(int offset, int count, CancellationToken cancellation);
        public Task<UserDetailsClientResponse> GetByIdAsync(Guid id, CancellationToken cancellation);
        public Task<Guid> RegisterAsync(UserRegisterClientRequest registerClientRequest, CancellationToken cancellation);
        public Task<TokenResponse> LoginAsync(UserLoginClientRequest loginClientRequest, CancellationToken cancellation);

        public Task<UserDetailsClientResponse> UpdateAsync(Guid id, UserUpdateClientRequest updateClientRequest, CancellationToken cancellation);
        public Task DeleteAsync(Guid id, CancellationToken cancellation);
        public Task<string> GenerateEmailTokenAsync(UserGenerateEmailTokenClientRequest request, CancellationToken cancellation);
        public Task<string> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenClientRequest clientRequest, CancellationToken cancellation);
        public Task ChangeEmailAsync(UserChangeEmailClientRequest request, CancellationToken cancellationToken);

        public Task ConfirmEmailAsync(UserEmailConfirmClientRequest clientRequest, CancellationToken cancellation);

    }
}
