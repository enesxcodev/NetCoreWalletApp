using Application.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IUserRepository : IRepository<AppUser>
    {
        //username varmı yok mu ona bakalım
        Task<bool> ExistsUser(string username, CancellationToken cancellationToken = default);
        Task<Result<Guid>> RegisterAsync(AppUser appUser, string password, CancellationToken cancellationToken = default);
        Task<Result<string>> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
        string GenerateJwtToken(AppUser user);
    }
}
