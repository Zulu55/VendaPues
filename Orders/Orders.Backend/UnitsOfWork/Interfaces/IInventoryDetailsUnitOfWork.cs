﻿using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IInventoryDetailsUnitOfWork
    {
        Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail);

        Task<ActionResponse<int>> GetRecordsNumberCount1Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount1Async(PaginationDTO pagination);

        Task<ActionResponse<int>> GetRecordsNumberCount2Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount2Async(PaginationDTO pagination);

        Task<ActionResponse<int>> GetRecordsNumberCount3Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount3Async(PaginationDTO pagination);
    }
}
