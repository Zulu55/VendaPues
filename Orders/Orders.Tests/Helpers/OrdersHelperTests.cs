using Moq;
using Orders.Backend.Helpers;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Tests.Helpers
{
    [TestClass]
    public class OrdersHelperTests
    {
        private Mock<IUsersUnitOfWork> _usersUnitOfWorkMock = null!;
        private Mock<IKardexUnitOfWork> _kardexUnitOfWorkMock = null!;
        private Mock<ITemporalOrdersUnitOfWork> _temporalOrdersUoWMock = null!;
        private Mock<IProductsUnitOfWork> _productsUoWMock = null!;
        private Mock<IOrdersUnitOfWork> _ordersUoWMock = null!;
        private OrdersHelper _ordersHelper = null!;

        [TestInitialize]
        public void Initialize()
        {
            _usersUnitOfWorkMock = new Mock<IUsersUnitOfWork>();
            _temporalOrdersUoWMock = new Mock<ITemporalOrdersUnitOfWork>();
            _productsUoWMock = new Mock<IProductsUnitOfWork>();
            _ordersUoWMock = new Mock<IOrdersUnitOfWork>();
            _kardexUnitOfWorkMock = new Mock<IKardexUnitOfWork>();

            _ordersHelper = new OrdersHelper(
                _usersUnitOfWorkMock.Object,
                _temporalOrdersUoWMock.Object,
                _productsUoWMock.Object,
                _ordersUoWMock.Object,
                _kardexUnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_UserDoesNotExist_ReturnsFalseActionResponse()
        {
            // Arrange
            string email = "test@test.com";
            var orderDto = new OrderDTO { Remarks = "remarks" };

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, orderDto);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Usuario no válido", result.Message);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_TemporalOrdersNotFound_ReturnsFalseActionResponse()
        {
            // Arrange
            string email = "test@test.com";
            var orderDto = new OrderDTO { Remarks = "remarks" };
            var user = new User { Email = email };

            _usersUnitOfWorkMock.Setup(uh => uh.GetUserAsync(email))
                .ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new ActionResponse<IEnumerable<TemporalOrder>> { WasSuccess = false });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, orderDto);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("No hay detalle en la orden", result.Message);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_InventoryCheckFails_ReturnsFalseActionResponse()
        {
            // Arrange
            string email = "test@test.com";
            var orderDto = new OrderDTO { Remarks = "remarks" };
            var user = new User { Email = email };
            var temporalOrders = new List<TemporalOrder>
            {
                new TemporalOrder { Quantity = 5, Product = new Product { Id = 1, Name = "Product1", Stock = 3 } }
            };

            _usersUnitOfWorkMock.Setup(uh => uh.GetUserAsync(email))
                .ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new ActionResponse<IEnumerable<TemporalOrder>> { WasSuccess = true, Result = temporalOrders });
            _productsUoWMock.Setup(puw => puw.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ActionResponse<Product> { WasSuccess = true, Result = temporalOrders[0].Product });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, orderDto);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual($"Lo sentimos no tenemos existencias suficientes del producto {temporalOrders[0].Product!.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.", result.Message);
        }
    }
}
