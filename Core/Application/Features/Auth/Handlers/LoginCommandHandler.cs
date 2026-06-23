using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Handlers
{
    public class LoginCommandHandler(IUserRepository userRepository, ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.LoginAsync(request.UserName, request.Password, cancellationToken);
            logger.LogInformation(Messages.Wallet.UserLoggin(request.UserName));
            return result;
        }
    }
}
