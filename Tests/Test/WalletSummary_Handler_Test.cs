using Application.Common.Enums;
using Application.Contracts;
using Application.Features.Wallet.Queries;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Test;

public class WalletSummary_Handler_Test
{
    [Fact]
    public async Task Handle_ReturnsAuthenticatedUsersCurrentWallet()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IWalletRepository>();
        var context = new Mock<IUserContext>();
        context.Setup(x => x.UserId).Returns(userId);
        repository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet(userId, "WLT-TEST1234", 125.50m));

        var result = await new GetWalletSummaryHandler(repository.Object, context.Object)
            .Handle(new GetWalletSummaryQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("WLT-TEST1234");
        result.Data.Balance.Should().Be(125.50m);
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundWhenWalletHasNotBeenCreatedYet()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IWalletRepository>();
        var context = new Mock<IUserContext>();
        context.Setup(x => x.UserId).Returns(userId);
        repository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await new GetWalletSummaryHandler(repository.Object, context.Object)
            .Handle(new GetWalletSummaryQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
