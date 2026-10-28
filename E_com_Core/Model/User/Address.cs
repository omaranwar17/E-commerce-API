using System.ComponentModel.DataAnnotations.Schema;

namespace E_com_Core.Model.User
{
    public class Address : BaseEntity<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public string AppUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(AppUserId))]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}