// Il Customer info usato solo quando il user effettua il suo primo ordine.

namespace Audiophile.Domain.Models
{
    public class CustomerInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!; // relazione con il user
        public Cart? Cart { get; set; } // relazione con il cart
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
