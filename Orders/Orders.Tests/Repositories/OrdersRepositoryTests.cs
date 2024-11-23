using Microsoft.EntityFrameworkCore;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class OrdersRepositoryTests
    {
        private DataContext _context = null!;
        private DbContextOptions<DataContext> _options = null!;
        private OrdersRepository _repository = null!;
        private Mock<IUsersRepository> _mockUserRepository = null!;
        private Mock<IKardexRepository> _mockKardexRepository = null!;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=:memory:") // SQLite en memoria
                .Options;

            _context = new DataContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _mockUserRepository = new Mock<IUsersRepository>();
            _mockKardexRepository = new Mock<IKardexRepository>();
            _repository = new OrdersRepository(_context, _mockUserRepository.Object, _mockKardexRepository.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Database.CloseConnection();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_UserDoesNotExist_ReturnsFailedActionResponse()
        {
            // Act
            var response = await _repository.GetAsync("nonexistentuser@example.com", new PaginationDTO());

            // Assert
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual("Usuario no válido", response.Message);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_UserDoesNotExist_ReturnsFailedActionResponse()
        {
            // Act
            var response = await _repository.GetTotalPagesAsync("nonexistentuser@example.com", new PaginationDTO());

            // Assert
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual("Usuario no válido", response.Message);
        }
    }
}