using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.DTOs.Table;
using RestaurantManageSystem.Application.DTOs.Setting; // ← ADD THIS
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<User, LoginResponseDto>();

            // Category mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // MenuItem mappings
            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));
            CreateMap<CreateMenuItemDto, MenuItem>();
            CreateMap<UpdateMenuItemDto, MenuItem>();

            // Table mappings
            CreateMap<Table, TableDto>();
            CreateMap<CreateTableDto, Table>();
            CreateMap<UpdateTableDto, Table>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table.TableNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<CreateOrderDto, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name));
            CreateMap<CreateOrderItemDto, OrderItem>();

            // Setting mappings
            CreateMap<Setting, SettingDto>(); // ← ADD THIS
        }
    }
}