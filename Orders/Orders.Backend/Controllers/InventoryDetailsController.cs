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

        [HttpPut]
        public override async Task<IActionResult> PutAsync(InventoryDetail model)
        {
            var response = await _inventoryDetailsUnitOfWork.UpdateAsync(model);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("recordsNumberCount1")]
        public async Task<IActionResult> GetRecordsNumberCount1Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount1Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("Count1")]
        public async Task<IActionResult> GetCount1Async([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoryDetailsUnitOfWork.GetCount1Async(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}