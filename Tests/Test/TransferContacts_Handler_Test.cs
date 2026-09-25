using Application.Contracts;
using Application.Features.Wallet.Queries;
using FluentAssertions;
using Moq;

namespace Test;

public class TransferContacts_Handler_Test
{
    [Fact]
    public async Task Handle_SendsTrimmedSearchAndCurrentUserToReader()
    {
        var userId = Guid.NewGuid();
        var context = new Mock<IUserContext>();
        context.Setup(x => x.UserId).Returns(userId);
        var reader = new Mock<ITransferContactReader>();
        reader.Setup(x => x.SearchAsync(userId, "Ayşe", 2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransferContactsPage(
                [new TransferContactResult("Ayşe", "Yılmaz", "ayse", "WLT-12345678")], 11, 2, 10));

        var result = await new SearchTransferContactsHandler(reader.Object, context.Object)
            .Handle(new SearchTransferContactsQuery(" Ayşe ", 2, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().ContainSingle();
        result.Data.TotalCount.Should().Be(11);
        reader.Verify(x => x.SearchAsync(userId, "Ayşe", 2, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RejectsInvalidPageWithoutDatabaseCall()
    {
        var reader = new Mock<ITransferContactReader>();
        var context = new Mock<IUserContext>();
        var result = await new SearchTransferContactsHandler(reader.Object, context.Object)
            .Handle(new SearchTransferContactsQuery("test", 0, 10), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        reader.VerifyNoOtherCalls();
    }
}
