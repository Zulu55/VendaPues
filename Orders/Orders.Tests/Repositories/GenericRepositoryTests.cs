using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class GenericRepositoryTests
    {
        private DataContext _context = null!;
        private DbContextOptions<DataContext> _options = null!;
        private GenericRepository<Category> _repository = null!;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=:memory:") // Usar SQLite en memoria
                .Options;

            _context = new DataContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _repository = new GenericRepository<Category>(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Database.CloseConnection();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var testEntity = new Category { Name = "Test" };

            // Act
            var response = await _repository.AddAsync(testEntity);

            // Assert
            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("Test", response.Result.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            // Arrange
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();

            // Act
            var response = await _repository.DeleteAsync(testEntity.Id);

            // Assert
            Assert.IsTrue(response.WasSuccess);
        }

        [TestMethod]
        public async Task GetAsync_ById_ShouldReturnEntity()
        {
            // Arrange
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();

            // Act
            var response = await _repository.GetAsync(testEntity.Id);

            // Assert
            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("Test", response.Result.Name);
        }

        [TestMethod]
        public async Task GetAsync_Pagination_ShouldReturnEntities()
        {
            // Arrange
            await _context.Set<Category>().AddRangeAsync(new List<Category>
            {
                new Category { Name = "Test1" },
                new Category { Name = "Test2" },
                new Category { Name = "Test3" },
            });
            await _context.SaveChangesAsync();

            // Act
            var paginationDTO = new PaginationDTO { RecordsNumber = 2 };
            var response = await _repository.GetAsync(paginationDTO);

            // Assert
            Assert.IsTrue(response.WasSuccess);
            Assert.AreEqual(2, response.Result.Count());
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();
            testEntity.Name = "UpdatedTest";

            // Act
            var response = await _repository.UpdateAsync(testEntity);

            // Assert
            Assert.IsTrue(response.WasSuccess);
            Assert.AreEqual("UpdatedTest", response.Result.Name);
        }
    }
}
