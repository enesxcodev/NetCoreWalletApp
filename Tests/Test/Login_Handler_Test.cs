using Application.Common;
using Application.Contracts;
using Application.Features.Auth.Commands;
using Application.Features.Auth.Handlers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test
{
    public class Login_Handler_Test
    {
        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnTokenSuccessfully()
        {
            // 1. Arrange (Hazırlık)
            var mockUserRepo = new Mock<IUserRepository>();
            var mockLogger = new Mock<ILogger<LoginCommandHandler>>();
            var userInfo = new
            {
                First = "Ahmet",
                Last = "Yıldırım",
                UserName = "ahmetxyildirim",
                Email = "yildirimahmet@gmail.com",
                Pass = "Sifremiz!23"
            };
            var command = new LoginCommand(userInfo.UserName, userInfo.Pass);
            string fakeJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...fakeToken";

            // Giriş başarılı olunca repo geriye JWT Token dönecek şekilde ayarlıyoruz
            var successResult = Result<string>.Success(fakeJwtToken);
            mockUserRepo.Setup(r => r.LoginAsync(command.UserName, command.Password, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(successResult);

            var handler = new LoginCommandHandler(mockUserRepo.Object, mockLogger.Object);

            // 2. Act (Çalıştırma)
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert (Doğrulama)
            result.IsSuccess.Should().BeTrue(); // Giriş başarılı olmalı
            result.Data.Should().Be(fakeJwtToken); // Dönen data bizim fake token olmalı

            // Log atıldı mı kontrolü 
            mockUserRepo.Verify(r => r.LoginAsync(command.UserName, command.Password, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCredentials_ShouldReturnFailureResult()
        {
            // 1. Arrange (Hatalı Giriş Senaryosu)
            var mockUserRepo = new Mock<IUserRepository>();
            var mockLogger = new Mock<ILogger<LoginCommandHandler>>();

            var command = new LoginCommand("yanlis_user", "yanlis_sifre");

            // Kullanıcı adı veya şifre hatalıysa repo başarısız Result döner
            var failureResult = Result<string>.Failure("Kullanıcı adı veya şifre hatalı!");
            mockUserRepo.Setup(r => r.LoginAsync(command.UserName, command.Password, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(failureResult);

            var handler = new LoginCommandHandler(mockUserRepo.Object, mockLogger.Object);

            // 2. Act
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert
            result.IsSuccess.Should().BeFalse(); // Giriş başarısız olmalı
            result.Data.Should().BeNull(); // Data boş dönmeli
            result.Error.Should().Be("Kullanıcı adı veya şifre hatalı!"); // Hata mesajı eşleşmeli
        }
    }
}
