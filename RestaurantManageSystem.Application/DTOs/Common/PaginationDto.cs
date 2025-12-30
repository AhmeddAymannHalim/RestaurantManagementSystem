namespace RestaurantManageSystem.Application.DTOs.Common
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class PaginatedResultDto<T>
    {
        public List<T> Items { get; set; } = [];
        public PaginationDto Pagination { get; set; } = new();

        public static PaginatedResultDto<T> Create(List<T> items, int page, int pageSize, int totalRecords)
        {
            return new PaginatedResultDto<T>
            {
                Items = items,
                Pagination = new PaginationDto
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                }
            };
        }
    }
}