using Microsoft.AspNetCore.Identity;
using Moq;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork;

[TestClass]
public class UsersUnitOfWorkTests
{
    private readonly Mock<IGenericRepository<User>> _mockGenericRepository = new Mock<IGenericRepository<User>>();
    private readonly Mock<IUsersRepository> _mockUsersRepository = new Mock<IUsersRepository>();
    private UsersUnitOfWork _usersUnitOfWork = null!;

    [TestInitialize]
    public void SetUp()
    {
        _usersUnitOfWork = new UsersUnitOfWork(_mockGenericRepository.Object, _mockUsersRepository.Object);
    }

    [TestMethod]
    public async Task GetRecordsNumberAsync_CallsUsersRepository_ReturnsRecordsNumber()
    {
        // Arrange
        var pagination = new PaginationDTO { RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<int> { WasSuccess = true, Result = 5 };

        _mockUsersRepository.Setup(repo => repo.GetRecordsNumberAsync(pagination))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.GetRecordsNumberAsync(pagination);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.GetRecordsNumberAsync(pagination), Times.Once);
    }

    [TestMethod]
    public async Task GetAsync_CallsUsersRepository_ReturnsUsers()
    {
        // Arrange
        var pagination = new PaginationDTO();
        var expectedResponse = new ActionResponse<IEnumerable<User>>
        {
            WasSuccess = true,
            Result = new List<User> { new User { FirstName = "John" } }
        };

        _mockUsersRepository.Setup(repo => repo.GetAsync(pagination))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.GetAsync(pagination);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.GetAsync(pagination), Times.Once);
    }

    [TestMethod]
    public async Task GetTotalPagesAsync_CallsUsersRepository_ReturnsTotalPages()
    {
        // Arrange
        var pagination = new PaginationDTO();
        var expectedResponse = new ActionResponse<int> { WasSuccess = true, Result = 10 };

        _mockUsersRepository.Setup(repo => repo.GetTotalPagesAsync(pagination))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.GetTotalPagesAsync(pagination);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.GetTotalPagesAsync(pagination), Times.Once);
    }

    [TestMethod]
    public async Task GeneratePasswordResetTokenAsync_CallsUsersRepository_ReturnsToken()
    {
        // Arrange
        var user = new User();
        var expectedToken = "reset-token";

        _mockUsersRepository.Setup(repo => repo.GeneratePasswordResetTokenAsync(user))
                            .ReturnsAsync(expectedToken);

        // Act
        var result = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);

        // Assert
        Assert.AreEqual(expectedToken, result);
        _mockUsersRepository.Verify(repo => repo.GeneratePasswordResetTokenAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_CallsUsersRepository_ReturnsIdentityResult()
    {
        // Arrange
        var user = new User();
        var token = "token";
        var password = "newPassword123";
        var expectedResponse = IdentityResult.Success;

        _mockUsersRepository.Setup(repo => repo.ResetPasswordAsync(user, token, password))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.ResetPasswordAsync(user, token, password);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.ResetPasswordAsync(user, token, password), Times.Once);
    }

    [TestMethod]
    public async Task GenerateEmailConfirmationTokenAsync_CallsUsersRepository_ReturnsToken()
    {
        // Arrange
        var user = new User();
        var expectedToken = "email-token";

        _mockUsersRepository.Setup(repo => repo.GenerateEmailConfirmationTokenAsync(user))
                            .ReturnsAsync(expectedToken);

        // Act
        var result = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);

        // Assert
        Assert.AreEqual(expectedToken, result);
        _mockUsersRepository.Verify(repo => repo.GenerateEmailConfirmationTokenAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task ConfirmEmailAsync_CallsUsersRepository_ReturnsIdentityResult()
    {
        // Arrange
        var user = new User();
        var token = "email-token";
        var expectedResponse = IdentityResult.Success;

        _mockUsersRepository.Setup(repo => repo.ConfirmEmailAsync(user, token))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.ConfirmEmailAsync(user, token);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.ConfirmEmailAsync(user, token), Times.Once);
    }

    [TestMethod]
    public async Task AddUserAsync_CallsUsersRepository_ReturnsIdentityResult()
    {
        // Arrange
        var user = new User();
        var password = "password123";
        var expectedResponse = IdentityResult.Success;

        _mockUsersRepository.Setup(repo => repo.AddUserAsync(user, password))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.AddUserAsync(user, password);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.AddUserAsync(user, password), Times.Once);
    }

    [TestMethod]
    public async Task LoginAsync_CallsUsersRepository_ReturnsSignInResult()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "user@example.com", Password = "password123" };
        var expectedResponse = SignInResult.Success;

        _mockUsersRepository.Setup(repo => repo.LoginAsync(loginDto))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.LoginAsync(loginDto);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.LoginAsync(loginDto), Times.Once);
    }

    [TestMethod]
    public async Task LogoutAsync_CallsUsersRepository()
    {
        // Arrange
        _mockUsersRepository.Setup(repo => repo.LogoutAsync()).Returns(Task.CompletedTask);

        // Act
        await _usersUnitOfWork.LogoutAsync();

        // Assert
        _mockUsersRepository.Verify(repo => repo.LogoutAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetUserAsync_ByEmail_CallsUsersRepository_ReturnsUser()
    {
        // Arrange
        var email = "user@example.com";
        var expectedUser = new User { Email = email };

        _mockUsersRepository.Setup(repo => repo.GetUserAsync(email))
                            .ReturnsAsync(expectedUser);

        // Act
        var result = await _usersUnitOfWork.GetUserAsync(email);

        // Assert
        Assert.AreEqual(expectedUser, result);
        _mockUsersRepository.Verify(repo => repo.GetUserAsync(email), Times.Once);
    }

    [TestMethod]
    public async Task GetUserAsync_ById_CallsUsersRepository_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User { Id = userId.ToString() };

        _mockUsersRepository.Setup(repo => repo.GetUserAsync(userId))
                            .ReturnsAsync(expectedUser);

        // Act
        var result = await _usersUnitOfWork.GetUserAsync(userId);

        // Assert
        Assert.AreEqual(expectedUser, result);
        _mockUsersRepository.Verify(repo => repo.GetUserAsync(userId), Times.Once);
    }

    [TestMethod]
    public async Task ChangePasswordAsync_CallsUsersRepository_ReturnsIdentityResult()
    {
        // Arrange
        var user = new User();
        var currentPassword = "oldPassword123";
        var newPassword = "newPassword123";
        var expectedResponse = IdentityResult.Success;

        _mockUsersRepository.Setup(repo => repo.ChangePasswordAsync(user, currentPassword, newPassword))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _usersUnitOfWork.ChangePasswordAsync(user, currentPassword, newPassword);

        // Assert
        Assert.AreEqual(expectedResponse, result);
        _mockUsersRepository.Verify(repo => repo.ChangePasswordAsync(user, currentPassword, newPassword), Times.Once);
    }
}
