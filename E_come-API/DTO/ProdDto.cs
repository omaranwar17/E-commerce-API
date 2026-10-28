namespace E_come_API.DTO
{
    public class ProdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public string CategoryName { get; set; } // كاتيجوري من غير منتجات
        public List<PhotoDto> Photos { get; set; }
    }
}
