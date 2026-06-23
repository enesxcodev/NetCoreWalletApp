using Application.Common;
using Application.Contracts;
using Application.Features.Auth.Commands;
using Application.Features.Auth.Handlers;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
namespace Test
{
    public class Register_Handle_Test
    {
        [Fact]
        public async Task Handle_WhenValidRequest_ShouldRegisterUserSuccessfully()
        {
            // 1. Arrange (Hazırlık)
            var mockUserRepo = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateRegisterHandler>>();

            var expectedUserId = Guid.NewGuid();
            var userInfo = new
            {
                Id = expectedUserId,
                First = "Ahmet",
                Last = "Yıldırım",
                UserName = "ahmetxyildirim",
                Email = "yildirimahmet@gmail.com",
                Pass = "Sifremiz!23"
            };

            var command = new RegisterCreateCommand(userInfo.First, userInfo.Last, userInfo.Email, userInfo.UserName, userInfo.Pass);
            var fakeUser = new AppUser(expectedUserId, userInfo.UserName, userInfo.Last, userInfo.Email, userInfo.UserName);

            // Mock 1: Mapper çağrıldığında sahte AppUser nesnesini dönsün
            mockMapper.Setup(m => m.Map<AppUser>(command)).Returns(fakeUser);

            // Mock 2: Repo çağrıldığında geriye başarılı bir Result<Guid> dönsün
            var successResult = Result<Guid>.Success(expectedUserId);
            mockUserRepo.Setup(r => r.RegisterAsync(fakeUser, command.Password, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(successResult);

            var handler = new CreateRegisterHandler(mockUserRepo.Object, mockMapper.Object, mockLogger.Object);

            // 2. Act (Çalıştırma)
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert (Doğrulama)
            result.IsSuccess.Should().BeTrue(); // Kayıt başarılı olmalı
            result.Data.Should().Be(expectedUserId); // Dönen UserId beklediğimiz ID olmalı

            // Bonus Doğrulama: Repo metodu gerçekten 1 kere tetiklendi mi?
            mockUserRepo.Verify(r => r.RegisterAsync(fakeUser, command.Password, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
