using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Clients.Users
{
    public interface IUserClient
    {
        public Task<IReadOnlyCollection<UserSummaryClientResponse>> GetAll(int offset, int count, CancellationToken cancellation);
        public Task<UserDetailsClientResponse> GetById(Guid id, CancellationToken cancellation);

        public Task<UserDetailsClientResponse> GetCurrent(CancellationToken cancellation);
        public Task<Guid> Register(UserRegisterClientRequest registerClientRequest, CancellationToken cancellation);
        public Task<string> Login(UserLoginClientRequest loginClientRequest, CancellationToken cancellation);

        public Task<UserDetailsClientResponse> Update(Guid id, UserUpdateClientRequest updateClientRequest, CancellationToken cancellation);
        public Task Delete(Guid id, CancellationToken cancellation);
    }
}
