using Microsoft.EntityFrameworkCore;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class TemporalOrdersRepositoryTests
    {
        private TemporalOrdersRepository _repository = null!;
        private DataContext _context = null!;
        private Mock<IUsersRepository> _userRepositoryMock = null!;
        private DbContextOptions<DataContext> _options = null!;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=:memory:") // Usar SQLite en memoria
                .Options;

            _context = new DataContext(_options);
            _context.Database.OpenConnection(); // Abrir conexión explícitamente
            _context.Database.EnsureCreated(); // Asegurar que la base de datos se crea

            _userRepositoryMock = new Mock<IUsersRepository>();
            _repository = new TemporalOrdersRepository(_context, _userRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.CloseConnection(); // Cerrar conexión explícitamente
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddFullAsync_ValidUser_ReturnsError()
        {
            // Arrange
            var email = "test@example.com";
            var product = new Product { Id = 1, Name = "Some", Description = "Some" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var dto = new TemporalOrderDTO
            {
                ProductId = product.Id,
                Quantity = 1
            };

            // Act
            var result = await _repository.AddFullAsync(email, dto);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Usuario no existe", result.Message);
        }

        [TestMethod]
        public async Task AddFullAsync_InvalidProduct_ReturnsError()
        {
            // Arrange
            var email = "test@example.com";
            var dto = new TemporalOrderDTO
            {
                ProductId = 999,
                Quantity = 1
            };

            // Act
            var result = await _repository.AddFullAsync(email, dto);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Producto no existe", result.Message);
        }

        [TestMethod]
        public async Task GetCountAsync_UserWithNoOrders_ReturnsZero()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = await _repository.GetCountAsync(email);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(0, result.Result);
        }

        [TestMethod]
        public async Task PutFullAsync_OrderDoesNotExist_ReturnsErrorActionResponse()
        {
            // Arrange
            var updateDTO = new TemporalOrderDTO { Id = 99, Remarks = "New Remarks", Quantity = 10 };

            // Act
            var result = await _repository.PutFullAsync(updateDTO);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Registro no encontrado", result.Message);
        }
    }
}
