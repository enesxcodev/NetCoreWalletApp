using Application.Common;
using MediatR;
using System.Security.Cryptography.X509Certificates;

namespace Application.Features.Auth.Commands
{
    public record LoginCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginCommand()
        {
            
        }

        public LoginCommand(string UserName,string Password)
        {
            
        }
    }
}
