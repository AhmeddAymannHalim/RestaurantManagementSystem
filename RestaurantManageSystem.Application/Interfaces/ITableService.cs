using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Table;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface ITableService
    {
        Task<ResponseDto<List<TableDto>>> GetAllTablesAsync();
        Task<ResponseDto<List<TableDto>>> GetAvailableTablesAsync();
        Task<ResponseDto<TableDto>> GetTableByIdAsync(int id);
        Task<ResponseDto<TableDto>> CreateTableAsync(CreateTableDto dto);
        Task<ResponseDto<TableDto>> UpdateTableAsync(int id, UpdateTableDto dto);
        Task<ResponseDto<bool>> DeleteTableAsync(int id);
    }
}