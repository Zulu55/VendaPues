﻿using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface ISuppliersUnitOfWork
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Supplier>> GetComboAsync();
    }
}
