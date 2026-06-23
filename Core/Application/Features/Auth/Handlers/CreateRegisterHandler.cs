using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Features.Auth.Commands;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Handlers
{
    public class CreateRegisterHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<CreateRegisterHandler> logger

    ) : IRequestHandler<RegisterCreateCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterCreateCommand request, CancellationToken cancellationToken)
        {
            var appUser = mapper.Map<AppUser>(request);
            var result = await userRepository.RegisterAsync(appUser, request.Password, cancellationToken);
            logger.LogInformation(Messages.Wallet.UsernameRegistered(request.UserName));

            return result;
        }
    }
}
