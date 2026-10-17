using AutoMapper;
using E_com_Core.Model.Products;
using E_come_API.DTO;

namespace E_come_API.Mapping
{
    public class CategoryMapping :Profile
    {
        public CategoryMapping()
        {
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<EditCateDto, Category>().ReverseMap();
        }
    }
}
