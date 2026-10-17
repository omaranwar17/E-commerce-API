//using AutoMapper;
//using E_com_Core.Model.Products;
//using E_come_API.DTO;

//namespace E_come_API.Mapping
//{
//    public class ProductMapping : Profile
//    {
//        public ProductMapping()
//        {
//            // 1. من العميل للسيرفر (Add)
//            CreateMap<ProductDto, Product>().ReverseMap();

//            // 2. من العميل للسيرفر (Edit)
//            CreateMap<EditProductDto, Product>();

//            // 3. من السيرفر للعميل (Response)
//            CreateMap<Product, ProdDto>()
//                .ForMember(dest => dest.CategoryName,
//                           opt => opt.MapFrom(src => src.Categorys.Name))
//                .ForMember(dest => dest.Photos,
//                           opt => opt.MapFrom(src => src.Photos)).ReverseMap();

//            CreateMap<Category, EditCateDto>().ReverseMap();
//            CreateMap<Photo, PhotoDto>().ReverseMap();
//            CreateMap<AddPDtos, Product>()
//     .ForMember(dest => dest.Photos, opt => opt.Ignore())
//     .ReverseMap();
//        }
//    }
//}

using AutoMapper;
using E_com_Core.Model.Products;
using E_come_API.DTO;

namespace E_come_API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // Add
            CreateMap<AddPDtos, Product>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            CreateMap<EditProdDto, Product>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            // Edit
            CreateMap<EditProductDto, Product>();

            // Response
            CreateMap<Product, ProdDto>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Categorys.Name))
                .ForMember(dest => dest.Photos,
                           opt => opt.MapFrom(src => src.Photos))
                .ReverseMap();

            CreateMap<Category, EditCateDto>().ReverseMap();
            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}