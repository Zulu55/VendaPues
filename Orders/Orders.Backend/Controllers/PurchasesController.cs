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
    public class PurchasesController : GenericController<Purchase>
    {
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;

        public PurchasesController(IGenericUnitOfWork<Purchase> unitOfWork, IPurchaseUnitOfWork purchaseUnitOfWork) : base(unitOfWork)
        {
            _purchaseUnitOfWork = purchaseUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumber([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseUnitOfWork.GetRecordsNumber(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var action = await _purchaseUnitOfWork.GetAsync(id);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
    }
}