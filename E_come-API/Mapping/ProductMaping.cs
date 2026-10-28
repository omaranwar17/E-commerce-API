using AutoMapper;
using E_com_Core.Model.Products;
using E_come_API.DTO;

namespace E_come_API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // 1. Add (من AddPDtos لـ Product)
            CreateMap<AddPDtos, Product>()
                // بنحول الليستة لنص "S,M,L" قبل الحفظ
                .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src =>
                    src.Sizes != null && src.Sizes.Any()
                        ? string.Join(",", src.Sizes)
                        : null))
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            // 2. Edit (من EditProdDto لـ Product)
            CreateMap<EditProdDto, Product>()
                .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src =>
                    src.Sizes != null && src.Sizes.Any()
                        ? string.Join(",", src.Sizes)
                        : null))
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            // 3. Response (من Product لـ ProdDto)
            CreateMap<Product, ProdDto>()
                .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src => src.Sizes))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Categorys.Name))
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));

            // باقي المابينج
            CreateMap<Category, EditCateDto>().ReverseMap();
            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}