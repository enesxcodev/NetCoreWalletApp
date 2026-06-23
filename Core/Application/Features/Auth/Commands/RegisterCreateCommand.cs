using Application.Common;
using MediatR;

namespace Application.Features.Auth.Commands;

public record RegisterCreateCommand(string FirstName, string LastName, string Email, string UserName, string Password) : IRequest<Result<Guid>>;
