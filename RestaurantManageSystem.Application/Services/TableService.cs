using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Table;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Services
{
    public class TableService : ITableService
    {
        private readonly IRepository<Table> _tableRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TableService(IRepository<Table> tableRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<List<TableDto>>> GetAllTablesAsync()
        {
            try
            {
                var tables = await _tableRepository.GetAllAsync();

                var tableDtos = tables.Select(table =>
                {
                    var dto = _mapper.Map<TableDto>(table);

                    if (table.Orders != null && table.Orders.Any())
                    {
                        var activeOrder = table.Orders.FirstOrDefault();
                        if (activeOrder != null)
                        {
                            dto.CurrentOrderId = activeOrder.Id;
                            dto.CurrentOrderNumber = activeOrder.OrderNumber;
                        }
                    }

                    return dto;
                }).ToList();

                return ResponseDto<List<TableDto>>.SuccessResponse(tableDtos);
            }
            catch (Exception ex)
            {
                return ResponseDto<List<TableDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<TableDto>>> GetAvailableTablesAsync()
        {
            try
            {
                var tables = await _tableRepository.FindAsync(t => t.Status == TableStatus.Available && t.IsActive);
                var tableDtos = _mapper.Map<List<TableDto>>(tables);
                return ResponseDto<List<TableDto>>.SuccessResponse(tableDtos);
            }
            catch (Exception ex)
            {
                return ResponseDto<List<TableDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<TableDto>> GetTableByIdAsync(int id)
        {
            try
            {
                var table = await _tableRepository.GetByIdAsync(id);
                if (table == null)
                    return ResponseDto<TableDto>.FailureResponse("Table not found");

                var tableDto = _mapper.Map<TableDto>(table);

                // ✅ FIX: Handle null Orders collection
                if (table.Orders != null && table.Orders.Any())
                {
                    var activeOrder = table.Orders.FirstOrDefault();
                    if (activeOrder != null)
                    {
                        tableDto.CurrentOrderId = activeOrder.Id;
                        tableDto.CurrentOrderNumber = activeOrder.OrderNumber;
                    }
                }

                return ResponseDto<TableDto>.SuccessResponse(tableDto);
            }
            catch (Exception ex)
            {
                return ResponseDto<TableDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<TableDto>> CreateTableAsync(CreateTableDto dto)
        {
            try
            {
                var table = _mapper.Map<Table>(dto);
                await _tableRepository.AddAsync(table);
                await _unitOfWork.SaveChangesAsync();

                var tableDto = _mapper.Map<TableDto>(table);
                return ResponseDto<TableDto>.SuccessResponse(tableDto, "Table created successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<TableDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<TableDto>> UpdateTableAsync(int id, UpdateTableDto dto)
        {
            try
            {
                var table = await _tableRepository.GetByIdAsync(id);
                if (table == null)
                    return ResponseDto<TableDto>.FailureResponse("Table not found");

                _mapper.Map(dto, table);
                table.Id = id;

                await _tableRepository.UpdateAsync(table);
                await _unitOfWork.SaveChangesAsync();

                var tableDto = _mapper.Map<TableDto>(table);
                return ResponseDto<TableDto>.SuccessResponse(tableDto, "Table updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<TableDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteTableAsync(int id)
        {
            try
            {
                await _tableRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Table deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }
    }
}