﻿using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class InventoriesUnitOfWork : GenericUnitOfWork<Inventory>, IInventoriesUnitOfWork
    {
        private readonly IInventoriesRepository _inventoriesRepository;

        public InventoriesUnitOfWork(IGenericRepository<Inventory> repository, IInventoriesRepository inventoriesRepository) : base(repository)
        {
            _inventoriesRepository = inventoriesRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination) => await _inventoriesRepository.GetRecordsNumber(pagination);

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination) => await _inventoriesRepository.GetAsync(pagination);
    }
}