using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Table;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Services
{
    public class TableService(IUnitOfWork unitOfWork, IMapper mapper) : ITableService
    {
        public async Task<ResponseDto<List<TableDto>>> GetAllTablesAsync()
        {
            try
            {
                var tables = await unitOfWork.Tables.GetAllAsync();
                var tableDtos = mapper.Map<List<TableDto>>(tables);
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
                var tables = await unitOfWork.Tables.FindAsync(t => t.Status == TableStatus.Available && t.IsActive);
                var tableDtos = mapper.Map<List<TableDto>>(tables);
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
                var table = await unitOfWork.Tables.GetByIdAsync(id);
                if (table == null)
                    return ResponseDto<TableDto>.FailureResponse("Table not found");

                var tableDto = mapper.Map<TableDto>(table);
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
                var existingTables = await unitOfWork.Tables.FindAsync(t => t.TableNumber == dto.TableNumber);
                if (existingTables.Any())
                    return ResponseDto<TableDto>.FailureResponse("Table number already exists");

                var table = mapper.Map<Domain.Entities.Table>(dto);
                table.Status = TableStatus.Available;
                table.IsActive = true;
                table.CreatedAt = DateTime.UtcNow;

                await unitOfWork.Tables.AddAsync(table);
                await unitOfWork.SaveChangesAsync();

                var tableDto = mapper.Map<TableDto>(table);
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
                var table = await unitOfWork.Tables.GetByIdAsync(id);
                if (table == null)
                    return ResponseDto<TableDto>.FailureResponse("Table not found");

                var existingTables = await unitOfWork.Tables.FindAsync(t => t.TableNumber == dto.TableNumber && t.Id != id);
                if (existingTables.Any())
                    return ResponseDto<TableDto>.FailureResponse("Table number already exists");

                mapper.Map(dto, table);
                table.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.Tables.UpdateAsync(table);
                await unitOfWork.SaveChangesAsync();

                var tableDto = mapper.Map<TableDto>(table);
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
                var table = await unitOfWork.Tables.GetByIdAsync(id);
                if (table == null)
                    return ResponseDto<bool>.FailureResponse("Table not found");

                // Check for active orders
                var activeOrders = await unitOfWork.Orders.FindAsync(o =>
                    o.TableId == id &&
                    (o.Status == OrderStatus.Pending ||
                     o.Status == OrderStatus.Preparing ||
                     o.Status == OrderStatus.Ready));

                if (activeOrders.Any())
                    return ResponseDto<bool>.FailureResponse("Cannot delete table with active orders");

                await unitOfWork.Tables.DeleteAsync(id);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Table deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }
    }
}