using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class InventoryDetailsController : GenericController<InventoryDetail>
    {
        private readonly IInventoryDetailsUnitOfWork _inventoryDetailsUnitOfWork;

        public InventoryDetailsController(IGenericUnitOfWork<InventoryDetail> unitOfWork, IInventoryDetailsUnitOfWork inventoryDetailsUnitOfWork) : base(unitOfWork)
        {
            _inventoryDetailsUnitOfWork = inventoryDetailsUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumber([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetRecordsNumber(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}