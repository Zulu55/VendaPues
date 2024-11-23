using Microsoft.EntityFrameworkCore;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Implementations;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class ProductsRepositoryTests
    {
        private DataContext _context = null!;
        private ProductsRepository _repository = null!;
        private Mock<IFileStorage> _fileStorageMock = null!;
        private DbContextOptions<DataContext> _options = null!;

        private const string _string64base = "U29tZVZhbGlkQmFzZTY0U3RyaW5n";
        private const string _container = "products";

        [TestInitialize]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=:memory:") // Usar SQLite en memoria
                .Options;

            _context = new DataContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _fileStorageMock = new Mock<IFileStorage>();
            _repository = new ProductsRepository(_context, _fileStorageMock.Object);

            PopulateData();
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Database.CloseConnection();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddImagesAsync_ProductNotFound_ReturnsError()
        {
            var imageDto = new ImageDTO { ProductId = 999 };

            var result = await _repository.AddImageAsync(imageDto);

            Assert.IsFalse(result.WasSuccess);
        }

        [TestMethod]
        public async Task AddImageAsync_WithValidData_AddsImage()
        {
            var imageDTO = new ImageDTO
            {
                ProductId = 1,
                Images = new List<string> { _string64base }
            };

            _fileStorageMock.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container))
                .ReturnsAsync("storedImagePath");

            var result = await _repository.AddImageAsync(imageDTO);

            Assert.IsTrue(result.WasSuccess);
            Assert.IsTrue(result.Result!.Images[0].Contains("storedImagePath"));
            _fileStorageMock.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container), Times.Once());
        }

        [TestMethod]
        public async Task RemoveLastImageAsync_ProductNotFound_ReturnsError()
        {
            var imageDto = new ImageDTO { ProductId = 999 };

            var result = await _repository.RemoveLastImageAsync(imageDto);

            Assert.IsFalse(result.WasSuccess);
        }

        [TestMethod]
        public async Task RemoveLastImageAsync_RemovesLastImage_ReturnsOk()
        {
            var imagePath = "https://image2.jpg";
            _fileStorageMock.Setup(fs => fs.RemoveFileAsync(imagePath, _container))
                .Returns(Task.CompletedTask);

            var imageDto = new ImageDTO { ProductId = 2 };

            var result = await _repository.RemoveLastImageAsync(imageDto);

            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(1, result.Result!.Images.Count);
            _fileStorageMock.Verify(x => x.RemoveFileAsync(imagePath, _container), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_WithoutFilter_ReturnsAllProducts()
        {
            var pagination = new PaginationDTO { RecordsNumber = 10, Page = 1 };

            var result = await _repository.GetAsync(pagination);

            Assert.IsTrue(result.WasSuccess);
            var products = result.Result!.ToList();
            Assert.AreEqual(2, products.Count);
        }

        [TestMethod]
        public async Task AddFullAsync_ValidDTO_ReturnsOk()
        {
            _fileStorageMock.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container))
                .ReturnsAsync("testImage.jpg");

            var productDTO = new ProductDTO
            {
                Name = "TestProduct",
                Description = "Description",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { _string64base },
                ProductCategoryIds = new List<int> { 1 }
            };

            var result = await _repository.AddFullAsync(productDTO);

            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual("TestProduct", result.Result!.Name);
            _fileStorageMock.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container), Times.Once());
        }

        [TestMethod]
        public async Task AddFullAsync_DuplicateName_ReturnsErrors()
        {
            var productDTO = new ProductDTO
            {
                Name = "Product A",
                Description = "Product A",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { _string64base },
                ProductCategoryIds = new List<int> { 1 }
            };

            var result = await _repository.AddFullAsync(productDTO);

            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Ya existe un producto con el mismo nombre.", result.Message);
        }

        [TestMethod]
        public async Task UpdateFullAsync_ValidDTO_UpdatesProduct()
        {
            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 200.00M,
                Stock = 20,
                ProductCategoryIds = new List<int> { 2 }
            };

            var result = await _repository.UpdateFullAsync(productDTO);

            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual("Updated Name", result.Result!.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingItem_ReturnsSuccessResponse()
        {
            int id = 2;

            var response = await _repository.DeleteAsync(id);

            Assert.IsTrue(response.WasSuccess);
        }

        private void PopulateData()
        {
            var category1 = new Category { Id = 1, Name = "Category1" };
            var category2 = new Category { Id = 2, Name = "Category2" };
            _context.Categories.AddRange(category1, category2);
            _context.SaveChanges();

            var product1 = new Product
            {
                Id = 1,
                Name = "Product A",
                Description = "Product A",
                ProductCategories = new List<ProductCategory> { new ProductCategory { Category = category1 } }
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product B",
                Description = "Product B",
                ProductCategories = new List<ProductCategory> { new ProductCategory { Category = category1 } },
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Image = "https://image1.jpg" },
                    new ProductImage { Image = "https://image2.jpg" }
                }
            };
            _context.Products.AddRange(product1, product2);
            _context.SaveChanges();
        }
    }
}