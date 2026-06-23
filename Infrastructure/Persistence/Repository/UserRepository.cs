using Application.Common;
using Application.Common.Enums;
using Application.Common.Events;
using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using Persistence.Identity;
using System.Security.Claims;
using System.Text;

namespace Persistence.Repository
{
    public class UserRepository(
        AppDbContext context,
        UserManager<AppIdentityUser> userManager,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IMessageBus messageBus
    ) : GenericRepository<AppUser>(context), IUserRepository
    {
        private readonly UserManager<AppIdentityUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        public async Task<bool> ExistsUser(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<Result<Guid>> RegisterAsync(AppUser appUser, string password, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Identity tablosuna ekle
                var checkEmail = await _userManager.FindByEmailAsync(appUser.Email);
                if (checkEmail is not null)
                    return Result<Guid>.Failure("Aynı E-posta zaten kayıtlı");

                var identityUser = new AppIdentityUser { Id = appUser.Id, UserName = appUser.UserName, Email = appUser.Email };
                var identityResult = await _userManager.CreateAsync(identityUser, password);

                if (!identityResult.Succeeded)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken); // Hata varsa geri al
                    return Result<Guid>.Failure(identityResult.Errors.Select(e => e.Description).ToList());
                }

                await AddAsync(appUser, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var userEvent = new UserRegisteredEvent(appUser.Id);
                await messageBus.PublishAsync(userEvent, "user-registered-queue", cancellationToken);
                return Result<Guid>.Success(appUser.Id, ResultStatus.Created);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken); // Beklenmedik hata durumunda geri al
                return Result<Guid>.Failure("Kayıt esnasında sistemsel bir hata oluştu.", ResultStatus.Error);
            }
        }
        public async Task<Result<string>> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            var hasUser = await _userManager.FindByNameAsync(username);
            if (hasUser is null)
                return Result<string>.Failure("Kullanıcı adı veya şifre hatalıdır...", ResultStatus.BadRequest);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(hasUser, password);
            if (!isPasswordCorrect)
                return Result<string>.Failure("Kullanıcı adı veya şifre hatalıdır...", ResultStatus.BadRequest);

            var domainUser = new AppUser(hasUser.Id, "", "", hasUser.Email, hasUser.UserName);
            string token = GenerateJwtToken(domainUser);

            return Result<string>.Success($"{token}", ResultStatus.Ok);
        }

        public string GenerateJwtToken(AppUser user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Token içinde taşınacak kullanıcı bilgileri (Claims)
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
