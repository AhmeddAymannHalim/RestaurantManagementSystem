using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.DTOs.Table;
using RestaurantManageSystem.Application.DTOs.Setting;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

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
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.CategoryNameAr, opt => opt.MapFrom(src => src.Category.CategoryNameAr));
            CreateMap<CreateMenuItemDto, MenuItem>();
            CreateMap<UpdateMenuItemDto, MenuItem>();

            // Table mappings
            CreateMap<Table, TableDto>();
            CreateMap<CreateTableDto, Table>();
            CreateMap<UpdateTableDto, Table>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table.TableNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateOrderDto, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(dest => dest.MenuItemNameAr, opt => opt.MapFrom(src => src.MenuItem.NameAr));
            CreateMap<CreateOrderItemDto, OrderItem>();

            // Setting mappings
            CreateMap<Setting, SettingDto>();
        }
    }
}