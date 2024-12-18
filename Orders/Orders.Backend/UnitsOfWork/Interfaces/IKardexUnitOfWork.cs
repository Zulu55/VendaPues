﻿using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IKardexUnitOfWork
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination);
    }
}
