﻿using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface ITemporalPurchasesUnitOfWork
    {
        Task<ActionResponse<TemporalPurchase>> GetAsync(int id);

        Task<ActionResponse<TemporalPurchase>> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO);

        Task<ActionResponse<TemporalPurchaseDTO>> AddFullAsync(string email, TemporalPurchaseDTO temporalPurchaseDTO);

        Task<ActionResponse<IEnumerable<TemporalPurchase>>> GetAsync(string email);

        Task<ActionResponse<int>> GetCountAsync(string email);
    }
}