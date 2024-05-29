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
    public class InventoriesController : GenericController<Inventory>
    {
        private readonly IInventoriesUnitOfWork _inventoriesUnitOfWork;

        public InventoriesController(IGenericUnitOfWork<Inventory> unitOfWork, IInventoriesUnitOfWork inventoriesUnitOfWork) : base(unitOfWork)
        {
            _inventoriesUnitOfWork = inventoriesUnitOfWork;
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var response = await _inventoriesUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }

        [HttpPost]
        public override async Task<IActionResult> PostAsync(Inventory inventory)
        {
            var action = await _inventoriesUnitOfWork.AddAsync(inventory);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound();
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumber([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoriesUnitOfWork.GetRecordsNumber(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _inventoriesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}