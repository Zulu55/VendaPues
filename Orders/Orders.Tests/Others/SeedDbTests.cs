using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.UnitsOfWork.Interfaces;

namespace Orders.Tests.Others
{
    [TestClass]
    public class SeedDbTests
    {
        private SeedDb _seedDb = null!;
        private Mock<IUsersUnitOfWork> _usersUnitOfWorkMock = null!;
        private Mock<IFileStorage> _fileStorageMock = null!;
        private Mock<IRuntimeInformationWrapper> _runtimeInformationMock = null!;
        private DataContext _context = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "OrdersDbTest")
                .Options;
            _context = new DataContext(options);

            _usersUnitOfWorkMock = new Mock<IUsersUnitOfWork>();
            _fileStorageMock = new Mock<IFileStorage>();
            _runtimeInformationMock = new Mock<IRuntimeInformationWrapper>();

            _seedDb = new SeedDb(_context, _usersUnitOfWorkMock.Object, _fileStorageMock.Object, _runtimeInformationMock.Object);
        }

        [TestMethod]
        public async Task SeedAsync_ShouldSeedData()
        {
            // Arrange
            _runtimeInformationMock.Setup(r => r.IsOSPlatform(OSPlatform.Windows))
                .Returns(true);
            _fileStorageMock.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("imageUrl");

            // Act
            await _seedDb.SeedAsync();

            // Assert
            Assert.IsTrue(await _context.Countries.AnyAsync());
            Assert.IsTrue(await _context.Categories.AnyAsync());
            Assert.IsTrue(await _context.Products.AnyAsync());
            Assert.IsTrue(await _context.ProductCategories.AnyAsync());
            Assert.IsTrue(await _context.ProductImages.AnyAsync());
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}